using System.Collections.Generic;
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
    /// <see cref="SetInput"/>.</summary>
    internal class CropMachine : IMachine
    {
        /// <summary>Produce that has been picked but not yet stored, keyed by location and tile.
        ///
        /// A real machine keeps its output in <c>heldObject</c> until a chest takes it. This one
        /// has nowhere to put it, and Automate is allowed to store a stack only partially — so
        /// without somewhere to hold the remainder, the leftovers vanish and the crop is picked
        /// again next tick. The pending item lives here until the last of it is stored.
        ///
        /// Static because Automate rebuilds machine instances when it rescans a group; the buffer
        /// has to outlive the instance.</summary>
        private static readonly Dictionary<string, Item> Pending = new();

        private readonly HoeDirt Dirt;
        private readonly Vector2 Tile;
        private readonly string Key;

        public GameLocation Location { get; }
        public Rectangle TileArea { get; }
        public string MachineTypeID => "LazyMug.AutomateCrops/Crop";

        public CropMachine(HoeDirt dirt, GameLocation location, Vector2 tile)
        {
            Dirt = dirt;
            Location = location;
            Tile = tile;
            Key = $"{location.NameOrUniqueName}:{(int)tile.X},{(int)tile.Y}";
            TileArea = new Rectangle((int)tile.X, (int)tile.Y, 1, 1);
        }

        /// <summary>Drop any buffered produce, e.g. when changing save.</summary>
        public static void ClearPending() => Pending.Clear();

        public MachineState GetState()
        {
            if (!ModEntry.Config.Enabled)
                return MachineState.Disabled;

            // produce still waiting to be stored takes priority over the tile's own state
            if (Pending.ContainsKey(Key))
                return MachineState.Done;

            if (Dirt.crop == null)
                return ModEntry.Config.Replant ? MachineState.Empty : MachineState.Disabled;

            if (Dirt.crop.dead.Value || !ModEntry.IsHarvestedType(Dirt.crop))
                return MachineState.Disabled;

            return CropHarvest.IsReady(Dirt) ? MachineState.Done : MachineState.Processing;
        }

        public ITrackedStack? GetOutput()
        {
            // Pick once, then serve from the buffer until it's empty. Harvesting on every call
            // would re-roll (and duplicate) produce whenever a chest only took part of a stack.
            if (!Pending.TryGetValue(Key, out var produce))
            {
                if (!CropHarvest.IsReady(Dirt) || !ModEntry.IsHarvestedType(Dirt.crop))
                    return null;

                var who = ModEntry.HarvestingFarmer;
                produce = CropHarvest.Pick(Dirt, Tile, who, out int xp, out Item? bonus);
                if (produce == null || produce.Stack < 1)
                    return null;

                // The crop is consumed now, not in the callback: the produce is safe in the
                // buffer, so the tile can move on even if the chests are full this tick.
                if (ModEntry.Config.GrantExperience && xp > 0)
                    who.gainExperience(Farmer.farmingSkill, xp);

                CropHarvest.Finish(Dirt);

                if (bonus != null)
                    Location.debris.Add(new Debris(bonus, Tile * Game1.tileSize));

                Pending[Key] = produce;
            }

            return new TrackedItem(produce)
                .OnReduced((_, item) =>
                {
                    // keep the buffer in step with what's actually left
                    if (item == null || item.Stack < 1)
                        Pending.Remove(Key);
                    else
                        Pending[Key] = item;
                })
                .OnEmpty((_, _) => Pending.Remove(Key));
        }

        /// <summary>Plant a seed from the connected chests into this now-bare tile.</summary>
        public bool SetInput(IStorage input)
        {
            if (!ModEntry.Config.Replant || Dirt.crop != null || Pending.ContainsKey(Key))
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
