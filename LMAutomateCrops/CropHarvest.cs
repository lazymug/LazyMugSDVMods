using System;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.GameData.Crops;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using Object = StardewValley.Object;

namespace LMAutomateCrops
{
    /// <summary>Harvesting a crop the way the game does, but delivering the produce to the caller
    /// instead of to a farmer's inventory.
    ///
    /// <see cref="Crop.harvest"/> can't be reused directly: it pushes the item into
    /// <c>Game1.player</c>'s inventory, plays animations and sets <c>canMove = false</c> — none of
    /// which suits a machine running in the background. The quality roll, stack size and
    /// experience formula below are therefore copied from it, so an automated harvest is worth
    /// exactly what a manual one would be.</summary>
    internal static class CropHarvest
    {
        /// <summary>Whether this dirt holds a living crop that is ready to be picked.</summary>
        public static bool IsReady(HoeDirt dirt)
        {
            var crop = dirt?.crop;
            if (crop == null || crop.dead.Value || crop.forageCrop.Value)
                return false;   // forage crops are a different (and interactive) harvest

            if (string.IsNullOrWhiteSpace(crop.indexOfHarvest.Value))
                return false;

            return crop.currentPhase.Value >= crop.phaseDays.Count - 1
                   && (!crop.fullyGrown.Value || crop.dayOfCurrentPhase.Value <= 0);
        }

        /// <summary>Roll the produce for one harvest, matching Crop.harvest.</summary>
        public static Item? Pick(HoeDirt dirt, Vector2 tile, Farmer who, out int extraXp, out Item? bonusDrop)
        {
            extraXp = 0;
            bonusDrop = null;

            var crop = dirt.crop;
            if (crop == null)
                return null;

            CropData? data = crop.GetData();
            var random = Utility.CreateRandom(tile.X * 7.0, tile.Y * 11.0, Game1.stats.DaysPlayed, Game1.uniqueIDForThisGame);

            int quality = RollQuality(dirt, who, data, random);
            int stack = RollStack(data, who, random);

            Item item = crop.programColored.Value
                ? new ColoredObject(crop.indexOfHarvest.Value, 1, crop.tintColor.Value) { Quality = quality }
                : ItemRegistry.Create(crop.indexOfHarvest.Value, 1, quality);

            // the same lucky double-harvest the game rolls
            if (random.NextDouble() < who.team.AverageLuckLevel() / 1500.0 + who.team.AverageDailyLuck() / 1200.0 + 0.0001)
                stack *= 2;

            item.Stack = stack;

            // wheat gives hay, fibre gives mixed seeds — both rolled the same way
            switch (crop.indexOfHarvest.Value)
            {
                case "262" when random.NextDouble() < 0.4:
                    bonusDrop = ItemRegistry.Create("(O)178");
                    break;
                case "771" when random.NextDouble() < 0.1:
                    bonusDrop = ItemRegistry.Create("(O)770");
                    break;
            }

            extraXp = ExperienceFor(item);
            return item;
        }

        /// <summary>Farming experience for the produce: 16 * ln(0.018 * price + 1), as in Crop.harvest.</summary>
        public static int ExperienceFor(Item item)
        {
            int price = item is Object obj ? obj.Price : 0;
            return (int)Math.Round(16.0 * Math.Log(0.018 * price + 1.0, Math.E));
        }

        /// <summary>Reset a regrowing crop, or clear the dirt for one that doesn't come back.</summary>
        /// <returns>True if the crop was removed entirely (the tile is now bare).</returns>
        public static bool Finish(HoeDirt dirt)
        {
            var crop = dirt.crop;
            if (crop == null)
                return false;

            int regrow = crop.GetData()?.RegrowDays ?? -1;
            if (regrow > 0)
            {
                crop.fullyGrown.Value = true;
                crop.dayOfCurrentPhase.Value = regrow;
                return false;
            }

            dirt.crop = null;
            return true;
        }

        private static int RollQuality(HoeDirt dirt, Farmer who, CropData? data, Random random)
        {
            int fertiliser = dirt.GetFertilizerQualityBoostLevel();
            double chance = 0.2 * (who.FarmingLevel / 10.0)
                            + 0.2 * fertiliser * ((who.FarmingLevel + 2.0) / 12.0)
                            + 0.01;
            double silverChance = Math.Min(0.75, chance * 2.0);

            int quality = 0;
            if (fertiliser >= 3 && random.NextDouble() < chance / 2.0)
                quality = 4;
            else if (random.NextDouble() < chance)
                quality = 2;
            else if (random.NextDouble() < silverChance || fertiliser >= 3)
                quality = 1;

            return Math.Clamp(quality, data?.HarvestMinQuality ?? 0, data?.HarvestMaxQuality ?? quality);
        }

        private static int RollStack(CropData? data, Farmer who, Random random)
        {
            int stack = 1;
            if (data != null)
            {
                int min = data.HarvestMinStack;
                int max = Math.Max(min, data.HarvestMaxStack);
                if (data.HarvestMaxIncreasePerFarmingLevel > 0f)
                    max += (int)(who.FarmingLevel * data.HarvestMaxIncreasePerFarmingLevel);

                if (min > 1 || max > 1)
                    stack = random.Next(min, max + 1);

                if (data.ExtraHarvestChance > 0.0)
                {
                    while (random.NextDouble() < Math.Min(0.9, data.ExtraHarvestChance))
                        stack++;
                }
            }
            return stack;
        }
    }
}
