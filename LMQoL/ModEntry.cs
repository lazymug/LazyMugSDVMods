using System.Collections.Generic;
using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using LMQoL.Features.AutoGate;
using LMQoL.Features.MagnetRadiusForaging;
using LMQoL.Features.QuickStack;
using LMQoL.Features.BuildWithBags;
using LMQoL.Features.CharcoalKiln;
using LMQoL.Features.CookingWithBags;
using LMQoL.Features.ItemTotals;
using LMQoL.Features.PineNuts;
using LMQoL.Features.SellAnything;
using LMQoL.Features.SellPriceTooltip;
using LMQoL.Features.SiloCapacity;
using LMQoL.Features.SpeciesTooltip;

namespace LMQoL
{
    public class ModEntry : Mod
    {
        internal static ModConfig Config { get; private set; } = null!;

        /// <summary>Shared instance, for features that patch types resolved at runtime.</summary>
        internal static Harmony Harmony { get; private set; } = null!;

        private readonly List<IFeature> _features = new();
        private readonly SiloCapacityFeature _silo = new();
        private readonly CharcoalKilnFeature _kiln = new();
        private readonly PineNutsFeature _pineNuts = new();

        public override void Entry(IModHelper helper)
        {
            Config = helper.ReadConfig<ModConfig>();

            Harmony = new Harmony(ModManifest.UniqueID);
            Harmony.PatchAll();

            // Register features
            _features.Add(new AutoGateFeature());
            _features.Add(new MagnetRadiusForagingFeature());
            _features.Add(new QuickStackFeature());
            _features.Add(_silo);
            _features.Add(new SpeciesTooltipFeature());
            _features.Add(new BuildWithBagsFeature());
            _features.Add(new CookingWithBagsFeature());
            _features.Add(_kiln);
            _features.Add(_pineNuts);
            _features.Add(new ItemTotalsFeature());
            _features.Add(new SellAnythingFeature());
            _features.Add(new SellPriceTooltipFeature());

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
                    _kiln.Reapply();
                    _pineNuts.Reapply();
                    LMQoL.Features.SellPriceTooltip.MachineScanner.ClearCache();
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
                getValue: () => Config.SellPriceShowItemValue,
                setValue: v => Config.SellPriceShowItemValue = v,
                name: () => Helper.Translation.Get("sellprice.itemvalue").ToString(),
                tooltip: () => Helper.Translation.Get("sellprice.itemvalue.tooltip").ToString()
            );

            gmcm.AddBoolOption(
                mod: ModManifest,
                getValue: () => Config.SellPriceApplyProfessions,
                setValue: v => Config.SellPriceApplyProfessions = v,
                name: () => Helper.Translation.Get("sellprice.professions").ToString(),
                tooltip: () => Helper.Translation.Get("sellprice.professions.tooltip").ToString()
            );

            gmcm.AddBoolOption(
                mod: ModManifest,
                getValue: () => Config.SellPriceHighlightBest,
                setValue: v => Config.SellPriceHighlightBest = v,
                name: () => Helper.Translation.Get("sellprice.highlight").ToString(),
                tooltip: () => Helper.Translation.Get("sellprice.highlight.tooltip").ToString()
            );

            gmcm.AddBoolOption(
                mod: ModManifest,
                getValue: () => Config.SellPriceScanMachines,
                setValue: v => Config.SellPriceScanMachines = v,
                name: () => Helper.Translation.Get("sellprice.scan").ToString(),
                tooltip: () => Helper.Translation.Get("sellprice.scan.tooltip").ToString()
            );

            gmcm.AddBoolOption(
                mod: ModManifest,
                getValue: () => Config.SellPriceIncludeFood,
                setValue: v => Config.SellPriceIncludeFood = v,
                name: () => Helper.Translation.Get("sellprice.food").ToString(),
                tooltip: () => Helper.Translation.Get("sellprice.food.tooltip").ToString()
            );

            gmcm.AddNumberOption(
                mod: ModManifest,
                getValue: () => Config.SellPriceMaxOptions,
                setValue: v => Config.SellPriceMaxOptions = v,
                name: () => Helper.Translation.Get("sellprice.max").ToString(),
                tooltip: () => Helper.Translation.Get("sellprice.max.tooltip").ToString(),
                min: 1,
                max: 20,
                interval: 1
            );

            if (Helper.ModRegistry.IsLoaded("Cornucopia.ArtisanMachines"))
            {
                gmcm.AddBoolOption(
                    mod: ModManifest,
                    getValue: () => Config.SellPriceIncludeCornucopia,
                    setValue: v => Config.SellPriceIncludeCornucopia = v,
                    name: () => Helper.Translation.Get("sellprice.cornucopia").ToString(),
                    tooltip: () => Helper.Translation.Get("sellprice.cornucopia.tooltip").ToString()
                );
            }

            if (Helper.ModRegistry.IsLoaded("Wildflour.AtelierGoods"))
            {
                gmcm.AddBoolOption(
                    mod: ModManifest,
                    getValue: () => Config.SellPriceIncludeWildflour,
                    setValue: v => Config.SellPriceIncludeWildflour = v,
                    name: () => Helper.Translation.Get("sellprice.wildflour").ToString(),
                    tooltip: () => Helper.Translation.Get("sellprice.wildflour.tooltip").ToString()
                );
            }

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
                max: 48000,
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

            gmcm.AddBoolOption(
                mod: ModManifest,
                getValue: () => Config.SpeciesTooltipCrops,
                setValue: v => Config.SpeciesTooltipCrops = v,
                name: () => Helper.Translation.Get("species.crops").ToString(),
                tooltip: () => Helper.Translation.Get("species.crops.tooltip").ToString()
            );

            gmcm.AddBoolOption(
                mod: ModManifest,
                getValue: () => Config.SpeciesTooltipTrees,
                setValue: v => Config.SpeciesTooltipTrees = v,
                name: () => Helper.Translation.Get("species.trees").ToString(),
                tooltip: () => Helper.Translation.Get("species.trees.tooltip").ToString()
            );

            gmcm.AddBoolOption(
                mod: ModManifest,
                getValue: () => Config.SpeciesTooltipBushes,
                setValue: v => Config.SpeciesTooltipBushes = v,
                name: () => Helper.Translation.Get("species.bushes").ToString(),
                tooltip: () => Helper.Translation.Get("species.bushes.tooltip").ToString()
            );

            // --- Sell Anything ---
            gmcm.AddSectionTitle(
                mod: ModManifest,
                text: () => Helper.Translation.Get("section.sellany").ToString()
            );

            gmcm.AddBoolOption(
                mod: ModManifest,
                getValue: () => Config.SellAnythingEnabled,
                setValue: v => Config.SellAnythingEnabled = v,
                name: () => Helper.Translation.Get("sellany.enabled").ToString(),
                tooltip: () => Helper.Translation.Get("sellany.enabled.tooltip").ToString()
            );

            gmcm.AddBoolOption(
                mod: ModManifest,
                getValue: () => Config.SellAnythingShipping,
                setValue: v => Config.SellAnythingShipping = v,
                name: () => Helper.Translation.Get("sellany.shipping").ToString(),
                tooltip: () => Helper.Translation.Get("sellany.shipping.tooltip").ToString()
            );

            gmcm.AddBoolOption(
                mod: ModManifest,
                getValue: () => Config.SellAnythingShops,
                setValue: v => Config.SellAnythingShops = v,
                name: () => Helper.Translation.Get("sellany.shops").ToString(),
                tooltip: () => Helper.Translation.Get("sellany.shops.tooltip").ToString()
            );

            // --- Custom Charcoal Kiln ---
            gmcm.AddSectionTitle(
                mod: ModManifest,
                text: () => Helper.Translation.Get("section.kiln").ToString()
            );

            gmcm.AddBoolOption(
                mod: ModManifest,
                getValue: () => Config.CharcoalKilnEnabled,
                setValue: v => Config.CharcoalKilnEnabled = v,
                name: () => Helper.Translation.Get("kiln.enabled").ToString(),
                tooltip: () => Helper.Translation.Get("kiln.enabled.tooltip").ToString()
            );

            if (Helper.ModRegistry.IsLoaded("Cornucopia.ArtisanMachines"))
            {
                gmcm.AddBoolOption(
                    mod: ModManifest,
                    getValue: () => Config.PineNutsFromTreeSeedsEnabled,
                    setValue: v => Config.PineNutsFromTreeSeedsEnabled = v,
                    name: () => Helper.Translation.Get("pinenuts.enabled").ToString(),
                    tooltip: () => Helper.Translation.Get("pinenuts.enabled.tooltip").ToString()
                );
            }

            // --- Build With Bags (Item Bags integration) ---
            if (Helper.ModRegistry.IsLoaded("SlayerDharok.Item_Bags"))
            {
                gmcm.AddSectionTitle(
                    mod: ModManifest,
                    text: () => Helper.Translation.Get("section.buildbags").ToString()
                );

                gmcm.AddBoolOption(
                    mod: ModManifest,
                    getValue: () => Config.BuildWithBagsEnabled,
                    setValue: v => Config.BuildWithBagsEnabled = v,
                    name: () => Helper.Translation.Get("buildbags.enabled").ToString(),
                    tooltip: () => Helper.Translation.Get("buildbags.enabled.tooltip").ToString()
                );

                gmcm.AddBoolOption(
                    mod: ModManifest,
                    getValue: () => Config.ShopTradeWithBagsEnabled,
                    setValue: v => Config.ShopTradeWithBagsEnabled = v,
                    name: () => Helper.Translation.Get("shoptrade.enabled").ToString(),
                    tooltip: () => Helper.Translation.Get("shoptrade.enabled.tooltip").ToString()
                );

                if (Helper.ModRegistry.IsLoaded("blueberry.LoveOfCooking"))
                {
                    gmcm.AddBoolOption(
                        mod: ModManifest,
                        getValue: () => Config.CookWithBagsEnabled,
                        setValue: v => Config.CookWithBagsEnabled = v,
                        name: () => Helper.Translation.Get("cookbags.enabled").ToString(),
                        tooltip: () => Helper.Translation.Get("cookbags.enabled.tooltip").ToString()
                    );
                }

                gmcm.AddBoolOption(
                    mod: ModManifest,
                    getValue: () => Config.ItemTotalsEnabled,
                    setValue: v => Config.ItemTotalsEnabled = v,
                    name: () => Helper.Translation.Get("totals.enabled").ToString(),
                    tooltip: () => Helper.Translation.Get("totals.enabled.tooltip").ToString()
                );

                gmcm.AddBoolOption(
                    mod: ModManifest,
                    getValue: () => Config.ItemTotalsIncludeChests,
                    setValue: v => Config.ItemTotalsIncludeChests = v,
                    name: () => Helper.Translation.Get("totals.chests.option").ToString(),
                    tooltip: () => Helper.Translation.Get("totals.chests.option.tooltip").ToString()
                );

                gmcm.AddBoolOption(
                    mod: ModManifest,
                    getValue: () => Config.ItemTotalsIncludeBags,
                    setValue: v => Config.ItemTotalsIncludeBags = v,
                    name: () => Helper.Translation.Get("totals.bags.option").ToString(),
                    tooltip: () => Helper.Translation.Get("totals.bags.option.tooltip").ToString()
                );
            }
        }
    }
}
