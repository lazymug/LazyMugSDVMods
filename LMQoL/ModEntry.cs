using System.Collections.Generic;
using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using LMQoL.Features.AutoGate;
using LMQoL.Features.MagnetRadiusForaging;
using LMQoL.Features.SkipFade;
using LMQoL.Features.QuickStack;

namespace LMQoL
{
    public class ModEntry : Mod
    {
        internal static ModConfig Config { get; private set; } = null!;

        private readonly List<IFeature> _features = new();

        public override void Entry(IModHelper helper)
        {
            Config = helper.ReadConfig<ModConfig>();

            var harmony = new Harmony(ModManifest.UniqueID);
            harmony.PatchAll();

            // Register features
            _features.Add(new AutoGateFeature());
            _features.Add(new MagnetRadiusForagingFeature());
            _features.Add(new SkipFadeFeature());
            _features.Add(new QuickStackFeature());

            foreach (var feature in _features)
                feature.Register(helper, Monitor);

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

            // --- Auto Gate ---
            gmcm.AddSectionTitle(
                mod: ModManifest,
                text: () => Helper.Translation.Get("section.autogate").ToString()
            );

            gmcm.AddBoolOption(
                mod: ModManifest,
                getValue: () => Config.AutoGateEnabled,
                setValue: v => Config.AutoGateEnabled = v,
                name: () => Helper.Translation.Get("autogate.enabled").ToString()
            );

            gmcm.AddNumberOption(
                mod: ModManifest,
                getValue: () => Config.AutoGateCloseDelayTicks,
                setValue: v => Config.AutoGateCloseDelayTicks = v,
                name: () => Helper.Translation.Get("autogate.closedelay").ToString(),
                tooltip: () => Helper.Translation.Get("autogate.closedelay.tooltip").ToString(),
                min: 10,
                max: 300,
                interval: 10
            );

            // --- Magnet Radius Foraging ---
            gmcm.AddSectionTitle(
                mod: ModManifest,
                text: () => Helper.Translation.Get("section.magnet").ToString()
            );

            gmcm.AddBoolOption(
                mod: ModManifest,
                getValue: () => Config.MagnetForagingEnabled,
                setValue: v => Config.MagnetForagingEnabled = v,
                name: () => Helper.Translation.Get("magnet.enabled").ToString()
            );

            gmcm.AddNumberOption(
                mod: ModManifest,
                getValue: () => Config.MagnetForagingRadius,
                setValue: v => Config.MagnetForagingRadius = v,
                name: () => Helper.Translation.Get("magnet.radius").ToString(),
                tooltip: () => Helper.Translation.Get("magnet.radius.tooltip").ToString(),
                min: 1,
                max: 15,
                interval: 1
            );

            gmcm.AddNumberOption(
                mod: ModManifest,
                getValue: () => Config.MagnetForagingSpeed,
                setValue: v => Config.MagnetForagingSpeed = v,
                name: () => Helper.Translation.Get("magnet.speed").ToString(),
                tooltip: () => Helper.Translation.Get("magnet.speed.tooltip").ToString(),
                min: 2,
                max: 20,
                interval: 1
            );

            // --- Sell Price Tooltip ---
            gmcm.AddSectionTitle(
                mod: ModManifest,
                text: () => Helper.Translation.Get("section.sellprice").ToString()
            );

            gmcm.AddBoolOption(
                mod: ModManifest,
                getValue: () => Config.SellPriceTooltipEnabled,
                setValue: v => Config.SellPriceTooltipEnabled = v,
                name: () => Helper.Translation.Get("sellprice.enabled").ToString()
            );

            gmcm.AddBoolOption(
                mod: ModManifest,
                getValue: () => Config.SellPriceShowArtisan,
                setValue: v => Config.SellPriceShowArtisan = v,
                name: () => Helper.Translation.Get("sellprice.showartisan").ToString(),
                tooltip: () => Helper.Translation.Get("sellprice.showartisan.tooltip").ToString()
            );

            gmcm.AddBoolOption(
                mod: ModManifest,
                getValue: () => Config.SellPriceHighlightBest,
                setValue: v => Config.SellPriceHighlightBest = v,
                name: () => Helper.Translation.Get("sellprice.highlight").ToString(),
                tooltip: () => Helper.Translation.Get("sellprice.highlight.tooltip").ToString()
            );

            // --- Skip Fade Transitions ---
            gmcm.AddSectionTitle(
                mod: ModManifest,
                text: () => Helper.Translation.Get("section.skipfade").ToString()
            );

            gmcm.AddBoolOption(
                mod: ModManifest,
                getValue: () => Config.SkipFadeEnabled,
                setValue: v => Config.SkipFadeEnabled = v,
                name: () => Helper.Translation.Get("skipfade.enabled").ToString(),
                tooltip: () => Helper.Translation.Get("skipfade.enabled.tooltip").ToString()
            );

            // --- Quick Stack to Nearby Chests ---
            gmcm.AddSectionTitle(
                mod: ModManifest,
                text: () => Helper.Translation.Get("section.quickstack").ToString()
            );

            gmcm.AddBoolOption(
                mod: ModManifest,
                getValue: () => Config.QuickStackEnabled,
                setValue: v => Config.QuickStackEnabled = v,
                name: () => Helper.Translation.Get("quickstack.enabled").ToString()
            );

            gmcm.AddKeybindList(
                mod: ModManifest,
                getValue: () => Config.QuickStackKey,
                setValue: v => Config.QuickStackKey = v,
                name: () => Helper.Translation.Get("quickstack.key").ToString(),
                tooltip: () => Helper.Translation.Get("quickstack.key.tooltip").ToString()
            );

            gmcm.AddNumberOption(
                mod: ModManifest,
                getValue: () => Config.QuickStackRadius,
                setValue: v => Config.QuickStackRadius = v,
                name: () => Helper.Translation.Get("quickstack.radius").ToString(),
                tooltip: () => Helper.Translation.Get("quickstack.radius.tooltip").ToString(),
                min: 1,
                max: 15,
                interval: 1
            );
        }
    }
}
