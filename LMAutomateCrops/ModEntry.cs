using System;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;

namespace LMAutomateCrops
{
    public class ModEntry : Mod
    {
        internal static ModConfig Config { get; private set; } = null!;

        /// <summary>Who gets the experience. Automate runs with nobody standing there, so the
        /// harvest is credited to the main player, the way a Junimo hut's owner is.</summary>
        internal static Farmer HarvestingFarmer => Game1.MasterPlayer ?? Game1.player;

        public override void Entry(IModHelper helper)
        {
            Config = helper.ReadConfig<ModConfig>();
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        }

        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            RegisterWithAutomate();
            RegisterConfigMenu();
        }

        private void RegisterWithAutomate()
        {
            var automate = Helper.ModRegistry.GetApi<Pathoschild.Stardew.Automate.IAutomateAPI>("Pathoschild.Automate");
            if (automate == null)
            {
                Monitor.Log("Automate's API could not be read, so crops won't be harvested automatically.", LogLevel.Error);
                return;
            }

            automate.AddFactory(new CropAutomationFactory());
            Monitor.Log("Registered crop harvesting with Automate.", LogLevel.Debug);
        }

        /// <summary>Whether crops in this location should be automated.</summary>
        internal static bool IsAllowedLocation(GameLocation location)
        {
            if (location == null)
                return false;

            if (location.IsGreenhouse)
                return Config.IncludeGreenhouse;

            if (location is IslandLocation || location.InIslandContext())
                return Config.IncludeGingerIsland;

            return true;
        }

        private void RegisterConfigMenu()
        {
            var gmcm = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (gmcm == null)
                return;

            gmcm.Register(ModManifest, () => Config = new ModConfig(), () => Helper.WriteConfig(Config));

            string T(string key) => Helper.Translation.Get(key);

            gmcm.AddBoolOption(ModManifest, () => Config.Enabled, v => Config.Enabled = v, () => T("config.enabled"), () => T("config.enabled.tooltip"));
            gmcm.AddBoolOption(ModManifest, () => Config.GrantExperience, v => Config.GrantExperience = v, () => T("config.xp"), () => T("config.xp.tooltip"));
            gmcm.AddBoolOption(ModManifest, () => Config.Replant, v => Config.Replant = v, () => T("config.replant"), () => T("config.replant.tooltip"));
            gmcm.AddBoolOption(ModManifest, () => Config.IncludeGreenhouse, v => Config.IncludeGreenhouse = v, () => T("config.greenhouse"));
            gmcm.AddBoolOption(ModManifest, () => Config.IncludeGingerIsland, v => Config.IncludeGingerIsland = v, () => T("config.island"));
        }
    }
}
