using System;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using Object = StardewValley.Object;

namespace LMQoL.Features.BulkGeodes
{
    /// <summary>Lets you tell Clint how many geodes to break instead of clicking once per geode.
    ///
    /// Press the key while holding a stack at Clint's counter and a number prompt appears, capped
    /// at what you can actually afford and carry. Each geode is then processed exactly as the
    /// menu would: 25g, the same treasure roll, the same stat counters and first-time cases.</summary>
    public class BulkGeodesFeature : IFeature
    {
        private const int GeodePrice = 25;

        private IMonitor _log = null!;

        public string Id => "BulkGeodes";

        public void Register(IModHelper helper, IMonitor monitor)
        {
            _log = monitor;
            helper.Events.Input.ButtonsChanged += OnButtonsChanged;
        }

        private void OnButtonsChanged(object? sender, ButtonsChangedEventArgs e)
        {
            var config = ModEntry.Config;
            if (!config.BulkGeodesEnabled || !Context.IsWorldReady)
                return;

            if (Game1.activeClickableMenu is not GeodeMenu menu || !config.BulkGeodesKey.JustPressed())
                return;

            var held = menu.heldItem;
            if (held == null || !Utility.IsGeode(held))
            {
                Game1.playSound("cancel");
                return;
            }

            int most = MaxProcessable(held);
            if (most <= 0)
            {
                Game1.playSound("cancel");
                return;
            }

            ModEntry.ModHelper.Input.SuppressActiveKeybinds(config.BulkGeodesKey);

            string prompt = ModEntry.ModHelper.Translation.Get("bulkgeodes.prompt", new { max = most });
            Game1.activeClickableMenu = new NumberSelectionMenu(
                prompt,
                (_, amount, _) =>
                {
                    Game1.activeClickableMenu = menu;   // back to Clint
                    Process(menu, Math.Clamp(amount, 1, MaxProcessable(menu.heldItem)));
                },
                price: GeodePrice,
                minValue: 1,
                maxValue: most,
                defaultNumber: most);
        }

        /// <summary>How many the player can break right now: what they hold, what they can pay
        /// for, and what they have room to carry.</summary>
        private static int MaxProcessable(Item? held)
        {
            if (held == null || !Utility.IsGeode(held))
                return 0;

            int affordable = Game1.player.Money / GeodePrice;
            // one slot is freed as the last geode leaves the stack, hence the +1 when it's the
            // final one — mirrors the menu's own inventory check
            int room = Math.Max(0, Game1.player.freeSpotsInInventory() - 1) + (held.Stack == 1 ? 1 : 0);

            return Math.Max(0, Math.Min(held.Stack, Math.Min(affordable, room)));
        }

        private void Process(GeodeMenu menu, int count)
        {
            int cracked = 0;

            try
            {
                for (int i = 0; i < count; i++)
                {
                    var held = menu.heldItem;
                    if (held == null || held.Stack <= 0 || Game1.player.Money < GeodePrice)
                        break;
                    if (Game1.player.freeSpotsInInventory() < 1)
                        break;

                    var treasure = RollTreasure(held);
                    if (treasure == null)
                        break;

                    Game1.player.Money -= GeodePrice;
                    Game1.stats.GeodesCracked++;
                    if (held.QualifiedItemId is "(O)MysteryBox" or "(O)GoldenMysteryBox")
                        Game1.stats.Increment("MysteryBoxesOpened");

                    menu.heldItem = held.ConsumeStack(1);
                    Game1.player.addItemToInventoryBool(treasure);
                    cracked++;
                }
            }
            catch (Exception ex)
            {
                _log.Log($"Bulk geode processing stopped early: {ex.Message}", LogLevel.Warn);
            }

            if (cracked > 0)
            {
                Game1.playSound("stoneCrack");
                Game1.addHUDMessage(new HUDMessage(
                    ModEntry.ModHelper.Translation.Get("bulkgeodes.done", new { count = cracked }),
                    HUDMessage.achievement_type));
            }
            else
            {
                Game1.playSound("cancel");
            }
        }

        /// <summary>The treasure for one geode, including the special cases the menu handles:
        /// the first golden coconut, and the guaranteed clay before the first artifact.</summary>
        private static Item? RollTreasure(Item geode)
        {
            if (geode.QualifiedItemId == "(O)791" && !Game1.netWorldState.Value.GoldenCoconutCracked)
            {
                Game1.netWorldState.Value.GoldenCoconutCracked = true;
                return ItemRegistry.Create("(O)73");
            }

            var treasure = Utility.getTreasureFromGeode(geode);
            if (treasure == null)
                return null;

            if (geode.QualifiedItemId != "(O)275"
                && treasure is Object { Type: "Arch" }
                && treasure is not Object { Type: "Minerals" }
                && !Game1.player.hasOrWillReceiveMail("artifactFound"))
            {
                return ItemRegistry.Create("(O)390", 5);
            }

            return treasure;
        }
    }
}
