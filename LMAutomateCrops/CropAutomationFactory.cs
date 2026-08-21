using Microsoft.Xna.Framework;
using Pathoschild.Stardew.Automate;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.TerrainFeatures;
using Object = StardewValley.Object;

namespace LMAutomateCrops
{
    /// <summary>Tells Automate which tilled tiles are worth connecting to.</summary>
    internal class CropAutomationFactory : IAutomationFactory
    {
        public IAutomatable? GetFor(TerrainFeature feature, GameLocation location, in Vector2 tile)
        {
            // Switched off means switched off: claim nothing, so Automate behaves exactly as it
            // would without this mod installed.
            if (!ModEntry.Config.Enabled)
                return null;

            if (feature is not HoeDirt dirt || !ModEntry.IsAllowedLocation(location))
                return null;

            // A bare tile is only interesting when replanting is on (Automate offers seeds to
            // machines reporting Empty) or when it still holds produce waiting for a chest.
            if (dirt.crop == null && !ModEntry.Config.Replant && !CropMachine.HasPending(location, tile))
                return null;

            return new CropMachine(dirt, location, tile);
        }

        public IAutomatable? GetFor(Object obj, GameLocation location, in Vector2 tile) => null;

        public IAutomatable? GetFor(Building building, GameLocation location, in Vector2 tile) => null;

        public IAutomatable? GetForTile(GameLocation location, in Vector2 tile) => null;
    }
}
