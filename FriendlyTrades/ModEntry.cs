using System.Collections.Generic;
using FriendlyTrades.Data;
using FriendlyTrades.Models;
using FriendlyTrades.Utils;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace FriendlyTrades
{
    public class ModEntry : Mod
    {
        internal static ModEntry Instance { get; private set; } = null!;
        internal static ModConfig Config { get; private set; } = null!;

        /// <summary>True only when the dedicated sell-only trade shop is open (blocks non-specialty items).</summary>
        internal static bool IsCustomTradeShop { get; set; }

        /// <summary>The active specialty for bonus calculation. Set for both custom and regular shops.</summary>
        internal static NpcSpecialty? CurrentSpecialty { get; set; }

        /// <summary>The NPC name for the active bonus.</summary>
        internal static string? CurrentNpcName { get; set; }

        private SpecialtyRegistry _specialtyRegistry = null!;
        private NPC? _pendingNpc;

        public override void Entry(IModHelper helper)
        {
            Instance = this;
            Config = helper.ReadConfig<ModConfig>();
            _specialtyRegistry = new SpecialtyRegistry();

            var harmony = new Harmony(ModManifest.UniqueID);
            harmony.PatchAll();

            helper.Events.Input.ButtonPressed += OnButtonPressed;
            helper.Events.Display.MenuChanged += OnMenuChanged;
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;


        }

        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            var gmcm = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (gmcm == null) return;

            gmcm.Register(
                mod: ModManifest,
                reset: () => Config = new ModConfig(),
                save: () => Helper.WriteConfig(Config)
            );

            gmcm.AddBoolOption(
                mod: ModManifest,
                getValue: () => Config.EnableMod,
                setValue: value => Config.EnableMod = value,
                name: () => Helper.Translation.Get("config.enablemod").ToString()
            );

            gmcm.AddNumberOption(
                mod: ModManifest,
                getValue: () => Config.BonusMultiplier,
                setValue: value => Config.BonusMultiplier = value,
                name: () => Helper.Translation.Get("config.bonusmultiplier").ToString(),
                tooltip: () => Helper.Translation.Get("config.bonusmultiplier.tooltip").ToString(),
                min: 0.5f,
                max: 2.0f,
                interval: 0.1f
            );

            gmcm.AddBoolOption(
                mod: ModManifest,
                getValue: () => Config.ShowBonusMessages,
                setValue: value => Config.ShowBonusMessages = value,
                name: () => Helper.Translation.Get("config.showbonusmessages").ToString()
            );
        }

        private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady || !Config.EnableMod)
                return;

            if (!e.Button.IsActionButton())
                return;

            if (Game1.activeClickableMenu != null || Game1.dialogueUp || Game1.eventUp || Game1.isFestival())
                return;

            if (Game1.player.ActiveObject != null)
                return;

            var npc = GetSpecialistNpcNearAction();
            if (npc == null)
                return;

            var specialty = _specialtyRegistry.GetSpecialty(npc.Name);
            if (specialty == null)
                return;

            // If we're in the NPC's shop location, don't intercept the click.
            // The vanilla shop will open normally, and OnMenuChanged will activate the bonus.
            if (!string.IsNullOrEmpty(specialty.ShopLocationName)
                && Game1.currentLocation.Name == specialty.ShopLocationName)
            {
                return;
            }

            // Outside the shop location: show trade dialogue
            Helper.Input.Suppress(e.Button);

            int bonusPercent = BonusCalculator.GetBonusPercent(npc.Name);
            string question = Helper.Translation.Get("dialogue.question", new
            {
                npc = npc.displayName,
                bonus = bonusPercent
            }).ToString();

            var responses = new Response[]
            {
                new Response("sell", Helper.Translation.Get("dialogue.sell").ToString()),
                new Response("chat", Helper.Translation.Get("dialogue.chat").ToString())
            };

            _pendingNpc = npc;

            Game1.currentLocation.createQuestionDialogue(
                question,
                responses,
                HandleDialogueResponse,
                npc
            );
        }

        private void HandleDialogueResponse(Farmer who, string answer)
        {
            if (answer == "sell" && _pendingNpc != null)
            {
                OpenTradeMenu(_pendingNpc);
            }
            else if (answer == "chat" && _pendingNpc != null)
            {
                Game1.drawDialogue(_pendingNpc);
            }

            _pendingNpc = null;
        }

        private void OpenTradeMenu(NPC npc)
        {
            var specialty = _specialtyRegistry.GetSpecialty(npc.Name);
            if (specialty == null) return;

            CurrentSpecialty = specialty;
            CurrentNpcName = npc.Name;
            IsCustomTradeShop = true;

            var stock = new Dictionary<ISalable, ItemStockInformation>();
            var shopMenu = new ShopMenu("FriendlyTrades_" + npc.Name, stock, 0, npc.Name);

            shopMenu.inventory.highlightMethod = item => specialty.AcceptsItem(item);

            Game1.activeClickableMenu = shopMenu;

            ShowBonusHudMessage(npc, specialty);
        }

        private void OnMenuChanged(object? sender, MenuChangedEventArgs e)
        {
            // When any active bonus shop closes, clear state
            if (CurrentSpecialty != null && e.OldMenu is ShopMenu && e.NewMenu == null)
            {
                IsCustomTradeShop = false;
                CurrentSpecialty = null;
                CurrentNpcName = null;
                return;
            }

            // When a regular NPC shop opens (and we don't already have an active bonus), activate bonus
            if (e.NewMenu is ShopMenu newShop && CurrentSpecialty == null && Config.EnableMod)
            {
                TryActivateRegularShopBonus(newShop);
            }
        }

        private void TryActivateRegularShopBonus(ShopMenu shopMenu)
        {
            string? shopId = shopMenu.ShopId;
            if (string.IsNullOrEmpty(shopId))
                return;

            var npcName = _specialtyRegistry.GetNpcNameByShopId(shopId);
            if (npcName == null)
                return;

            var specialty = _specialtyRegistry.GetSpecialty(npcName);
            if (specialty == null)
                return;

            CurrentSpecialty = specialty;
            CurrentNpcName = npcName;
            IsCustomTradeShop = false;

            var npc = Game1.currentLocation?.getCharacterFromName(npcName);
            if (npc != null)
                ShowBonusHudMessage(npc, specialty);
        }

        private void ShowBonusHudMessage(NPC npc, NpcSpecialty specialty)
        {
            if (!Config.ShowBonusMessages)
                return;

            int bonusPercent = BonusCalculator.GetBonusPercent(npc.Name);
            if (bonusPercent <= 0)
                return;

            string specialtyName = Helper.Translation.Get("specialty." + specialty.SpecialtyKey).ToString();
            string message = Helper.Translation.Get("shop.bonus", new
            {
                npc = npc.displayName,
                specialty = specialtyName,
                bonus = bonusPercent
            }).ToString();

            Game1.addHUDMessage(new HUDMessage(message, HUDMessage.newQuest_type));
        }

        private NPC? GetSpecialistNpcNearAction()
        {
            var grabTile = Game1.player.GetGrabTile();

            foreach (var character in Game1.currentLocation.characters)
            {
                if (character is NPC npc &&
                    !npc.IsInvisible &&
                    Vector2.Distance(npc.Tile, grabTile) < 1.5f &&
                    _specialtyRegistry.HasSpecialty(npc.Name))
                {
                    return npc;
                }
            }

            return null;
        }
    }
}
