using Microsoft.Xna.Framework;
using Pathoschild.Stardew.Automate;
using StardewValley;
using StardewValley.TerrainFeatures;
using Object = StardewValley.Object;

namespace LMAutomateCrops
{
    /// <summary>A tilled tile presented to Automate as a machine.
    ///
    /// It reports <c>Done</c> when the crop on it is ready to pick, and — when replanting is on —
    /// <c>Empty</c> once the tile is bare, which is how Automate knows to offer it seeds through
    /// <see cref="SetInput"/>. That's the same input path any other machine uses, so the seed is
    /// taken from a connected chest with no special handling.</summary>
    internal class CropMachine : IMachine
    {
        private readonly HoeDirt Dirt;
        private readonly Vector2 Tile;

        public GameLocation Location { get; }
        public Rectangle TileArea { get; }
        public string MachineTypeID => "LazyMug.AutomateCrops/Crop";

        public CropMachine(HoeDirt dirt, GameLocation location, Vector2 tile)
        {
            Dirt = dirt;
            Location = location;
            Tile = tile;
            TileArea = new Rectangle((int)tile.X, (int)tile.Y, 1, 1);
        }

        public MachineState GetState()
        {
            if (!ModEntry.Config.Enabled)
                return MachineState.Disabled;

            if (Dirt.crop == null)
                return ModEntry.Config.Replant ? MachineState.Empty : MachineState.Disabled;

            if (Dirt.crop.dead.Value)
                return MachineState.Disabled;

            return CropHarvest.IsReady(Dirt) ? MachineState.Done : MachineState.Processing;
        }

        public ITrackedStack? GetOutput()
        {
            if (!CropHarvest.IsReady(Dirt))
                return null;

            var who = ModEntry.HarvestingFarmer;
            var produce = CropHarvest.Pick(Dirt, Tile, who, out int xp, out Item? bonus);
            if (produce == null)
                return null;

            // The crop is only actually picked once Automate has moved the produce into a chest,
            // so clearing the tile and paying the experience happen in the callback.
            return new TrackedItem(produce).OnEmpty((_, _) =>
            {
                if (ModEntry.Config.GrantExperience && xp > 0)
                    who.gainExperience(Farmer.farmingSkill, xp);

                CropHarvest.Finish(Dirt);

                if (bonus != null)
                    Location.debris.Add(new Debris(bonus, Tile * Game1.tileSize));
            });
        }

        /// <summary>Plant a seed from the connected chests into this now-bare tile.</summary>
        public bool SetInput(IStorage input)
        {
            if (!ModEntry.Config.Replant || Dirt.crop != null)
                return false;

            foreach (var stack in input.GetItems())
            {
                if (stack?.Sample is not Object seed || seed.Category != Object.SeedsCategory)
                    continue;

                // only sow what can actually grow here and now, so seeds aren't wasted
                if (!Dirt.canPlantThisSeedHere(seed.ItemId))
                    continue;

                if (!Dirt.plant(seed.ItemId, ModEntry.HarvestingFarmer, isFertilizer: false))
                    continue;

                stack.Reduce(1);
                return true;
            }

            return false;
        }
    }
}
