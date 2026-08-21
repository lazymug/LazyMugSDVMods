using Microsoft.Xna.Framework;
using Pathoschild.Stardew.Automate;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.TerrainFeatures;
using Object = StardewValley.Object;

namespace LMAutomateCrops
{
    /// <summary>Tells Automate that tilled dirt with a crop on it is a machine worth connecting to.</summary>
    internal class CropAutomationFactory : IAutomationFactory
    {
        public IAutomatable? GetFor(TerrainFeature feature, GameLocation location, in Vector2 tile)
        {
            if (!ModEntry.Config.Enabled || feature is not HoeDirt dirt)
                return null;

            // A bare tile is still worth tracking: it may hold produce that hasn't been stored
            // yet, and when replanting is on Automate offers seeds to machines reporting Empty.
            if (dirt.crop == null && !ModEntry.Config.Replant)
                return new CropMachine(dirt, location, tile);

            if (!ModEntry.IsAllowedLocation(location))
                return null;

            return new CropMachine(dirt, location, tile);
        }

        public IAutomatable? GetFor(Object obj, GameLocation location, in Vector2 tile) => null;

        public IAutomatable? GetFor(Building building, GameLocation location, in Vector2 tile) => null;

        public IAutomatable? GetForTile(GameLocation location, in Vector2 tile) => null;
    }
}
