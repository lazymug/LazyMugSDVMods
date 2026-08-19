using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.GameData.Buildings;

namespace LMQoL.Features.SiloCapacity
{
    /// <summary>Overrides how much hay a silo holds.
    ///
    /// Capacity lives in <c>Data/Buildings</c> (<see cref="BuildingData.HayCapacity"/>), which the
    /// game copies into each building's <c>hayCapacity</c> when it loads or reloads its data, and
    /// then sums in <c>GameLocation.GetHayCapacity()</c>. Editing the asset therefore covers silos
    /// that already exist as well as new ones — no patching of the hay maths required.</summary>
    public class SiloCapacityFeature : IFeature
    {
        private const string SiloId = "Silo";   // vanilla capacity is 240

        private IModHelper _helper = null!;

        public string Id => "SiloCapacity";

        public void Register(IModHelper helper, IMonitor monitor)
        {
            _helper = helper;
            helper.Events.Content.AssetRequested += OnAssetRequested;
            // Each building serialises its own hayCapacity into the save and only re-reads
            // Data/Buildings when it's constructed, so silos from an existing save keep whatever
            // capacity they were built with until we push the current value into them.
            helper.Events.GameLoop.SaveLoaded += (_, _) => Reapply();
        }

        private void OnAssetRequested(object? sender, AssetRequestedEventArgs e)
        {
            if (!ModEntry.Config.SiloCapacityEnabled || !e.NameWithoutLocale.IsEquivalentTo("Data/Buildings"))
                return;

            e.Edit(asset =>
            {
                var data = asset.AsDictionary<string, BuildingData>().Data;
                if (data.TryGetValue(SiloId, out var silo))
                    silo.HayCapacity = ModEntry.Config.SiloCapacity;
            }, AssetEditPriority.Late);
        }

        /// <summary>Refresh the asset, then push the current capacity into silos that already
        /// exist. Called on save load and whenever the config is saved.</summary>
        public void Reapply()
        {
            _helper.GameContent.InvalidateCache("Data/Buildings");

            if (!Context.IsWorldReady)
                return;

            Utility.ForEachLocation(location =>
            {
                foreach (var building in location.buildings)
                {
                    if (building.buildingType.Value == SiloId)
                        building.ReloadBuildingData();
                }
                return true;
            });
        }
    }
}
