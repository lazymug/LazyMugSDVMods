using System;
using System.Collections.Generic;
using HarmonyLib;
using LMQoL.Features.BuildWithBags;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Inventories;
using StardewValley.Objects;
using Object = StardewValley.Object;

namespace LMQoL.Features.CookingWithBags
{
    /// <summary>Gives Love of Cooking's kitchen an extra "Item Bags" tab holding everything stored
    /// in the player's bags, so bagged ingredients can be cooked with.
    ///
    /// Unlike the carpenter and shop integrations, Love of Cooking doesn't go through a vanilla
    /// check/consume pair — it has its own CookingManager working over a list of inventories. So
    /// instead of patching the maths, the bags are offered to the menu as one more container and
    /// the result is written back when the menu closes.
    ///
    /// The tab is filled with COPIES, never the bag's own objects: the menu mutates and removes
    /// entries as it cooks, and doing that to the real objects would leave zero-stack ghosts
    /// inside the bag. The difference is settled afterwards through Item Bags' own API.</summary>
    public class CookingWithBagsFeature : IFeature
    {
        private const string LoveOfCookingId = "blueberry.LoveOfCooking";
        private const string CookingMenuTypeName = "LoveOfCooking.Menu.CookingMenu";

        /// <summary>The container handed to the menu, and what it held when we handed it over.</summary>
        private static Chest? _bagChest;
        private static Dictionary<string, int> _snapshot = new();
        private static IMonitor? _log;

        public string Id => "CookingWithBags";

        internal static bool Active
            => BuildWithBagsFeature.Api != null && ModEntry.Config.CookWithBagsEnabled;

        public void Register(IModHelper helper, IMonitor monitor)
        {
            _log = monitor;
            helper.Events.GameLoop.GameLaunched += (_, _) => Hook(helper, monitor);
            helper.Events.Display.MenuChanged += OnMenuChanged;
        }

        private static void Hook(IModHelper helper, IMonitor monitor)
        {
            if (!helper.ModRegistry.IsLoaded(LoveOfCookingId))
                return;

            // The menu type belongs to another mod, so it's resolved by name and patched manually —
            // a compile-time reference would stop LM QoL loading when that mod is absent.
            var menuType = AccessTools.TypeByName(CookingMenuTypeName);
            var ctor = menuType == null
                ? null
                : AccessTools.Constructor(menuType, new[]
                {
                    typeof(List<CraftingRecipe>), typeof(Dictionary<IInventory, Chest>), typeof(string)
                });

            if (ctor == null)
            {
                monitor.Log("Love of Cooking is installed but its cooking menu could not be found; cooking from bags is off.", LogLevel.Warn);
                return;
            }

            ModEntry.Harmony.Patch(ctor, prefix: new HarmonyMethod(typeof(CookingWithBagsFeature), nameof(AddBagsAsContainer)));
            monitor.Log("Love of Cooking detected — bagged ingredients will be offered in the kitchen.", LogLevel.Debug);
        }

        /// <summary>Adds the bag contents to the containers the kitchen may draw from.</summary>
        internal static void AddBagsAsContainer(ref Dictionary<IInventory, Chest>? materialContainers)
        {
            _bagChest = null;
            _snapshot = new Dictionary<string, int>();

            if (!Active)
                return;

            try
            {
                var api = BuildWithBagsFeature.Api!;
                var player = Game1.player;
                var chest = new Chest(playerChest: true);

                foreach (var bag in api.GetItemBags(player.Items))
                {
                    foreach (var stored in api.GetObjectsInsideBag(bag, includeNestedBags: true))
                    {
                        if (stored == null || stored.Stack <= 0)
                            continue;

                        var copy = (Object)stored.getOne();
                        copy.Stack = stored.Stack;
                        chest.Items.Add(copy);

                        _snapshot.TryGetValue(copy.QualifiedItemId, out int had);
                        _snapshot[copy.QualifiedItemId] = had + stored.Stack;
                    }
                }

                if (chest.Items.Count == 0)
                    return;

                materialContainers ??= new Dictionary<IInventory, Chest>();
                materialContainers[chest.Items] = chest;
                _bagChest = chest;
            }
            catch (Exception ex)
            {
                _bagChest = null;
                _log?.Log($"Could not offer Item Bags to the kitchen: {ex.Message}", LogLevel.Warn);
            }
        }

        private static void OnMenuChanged(object? sender, MenuChangedEventArgs e)
        {
            if (_bagChest == null || e.OldMenu?.GetType().FullName != CookingMenuTypeName)
                return;

            try
            {
                SettleUp();
            }
            catch (Exception ex)
            {
                _log?.Log($"Could not settle Item Bags after cooking: {ex.Message}", LogLevel.Error);
            }
            finally
            {
                _bagChest = null;
                _snapshot = new Dictionary<string, int>();
            }
        }

        /// <summary>Compare what's left in the tab with what we put in it, and make the bags match.</summary>
        private static void SettleUp()
        {
            var api = BuildWithBagsFeature.Api;
            if (api == null || _bagChest == null)
                return;

            var remaining = new Dictionary<string, int>();
            foreach (var item in _bagChest.Items)
            {
                if (item == null || item.Stack <= 0)
                    continue;
                remaining.TryGetValue(item.QualifiedItemId, out int had);
                remaining[item.QualifiedItemId] = had + item.Stack;
            }

            var player = Game1.player;

            foreach (var (itemId, started) in _snapshot)
            {
                remaining.TryGetValue(itemId, out int left);
                int used = started - left;
                if (used > 0)
                    BagContents.Take(api, player, itemId, used);   // cooked or moved out: remove from the bag
            }

            // Anything the player dropped into the tab isn't in the bag, so hand it back rather
            // than letting it vanish with the menu.
            foreach (var (itemId, left) in remaining)
            {
                _snapshot.TryGetValue(itemId, out int started);
                int gained = left - started;
                if (gained <= 0)
                    continue;

                var item = ItemRegistry.Create(itemId, gained, allowNull: true);
                if (item == null)
                    continue;

                var leftover = player.addItemToInventory(item);
                if (leftover != null)
                    Game1.createItemDebris(leftover, player.getStandingPosition(), player.FacingDirection);
            }
        }
    }
}
