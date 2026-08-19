using System.Collections.Generic;
using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using LMQoL.Features.AutoGate;
using LMQoL.Features.MagnetRadiusForaging;
using LMQoL.Features.QuickStack;
using LMQoL.Features.SiloCapacity;
using LMQoL.Features.SpeciesTooltip;

namespace LMQoL
{
    public class ModEntry : Mod
    {
        internal static ModConfig Config { get; private set; } = null!;

        private readonly List<IFeature> _features = new();
        private readonly SiloCapacityFeature _silo = new();

        public override void Entry(IModHelper helper)
        {
            Config = helper.ReadConfig<ModConfig>();

            var harmony = new Harmony(ModManifest.UniqueID);
            harmony.PatchAll();

            // Register features
            _features.Add(new AutoGateFeature());
            _features.Add(new MagnetRadiusForagingFeature());
            _features.Add(new QuickStackFeature());
            _features.Add(_silo);
            _features.Add(new SpeciesTooltipFeature());

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
                save: () =>
                {
                    Helper.WriteConfig(Config);
                    _silo.Reapply();
                }
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

            // --- Silo Capacity ---
            gmcm.AddSectionTitle(
                mod: ModManifest,
                text: () => Helper.Translation.Get("section.silo").ToString()
            );

            gmcm.AddBoolOption(
                mod: ModManifest,
                getValue: () => Config.SiloCapacityEnabled,
                setValue: v => Config.SiloCapacityEnabled = v,
                name: () => Helper.Translation.Get("silo.enabled").ToString(),
                tooltip: () => Helper.Translation.Get("silo.enabled.tooltip").ToString()
            );

            gmcm.AddNumberOption(
                mod: ModManifest,
                getValue: () => Config.SiloCapacity,
                setValue: v => Config.SiloCapacity = v,
                name: () => Helper.Translation.Get("silo.capacity").ToString(),
                tooltip: () => Helper.Translation.Get("silo.capacity.tooltip").ToString(),
                min: 240,
                max: 4800,
                interval: 240
            );

            // --- Species Tooltip ---
            gmcm.AddSectionTitle(
                mod: ModManifest,
                text: () => Helper.Translation.Get("section.species").ToString()
            );

            gmcm.AddBoolOption(
                mod: ModManifest,
                getValue: () => Config.SpeciesTooltipEnabled,
                setValue: v => Config.SpeciesTooltipEnabled = v,
                name: () => Helper.Translation.Get("species.enabled").ToString(),
                tooltip: () => Helper.Translation.Get("species.enabled.tooltip").ToString()
            );
        }
    }
}
