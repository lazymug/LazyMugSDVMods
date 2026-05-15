using System;
using System.Collections.Generic;
using Allellopathy.Models;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace Allellopathy.Utils
{
    public static class CropEffects
    {
        // SDV 1.6 fertilizer item IDs
        private const string BasicFertilizer = "368";
        private const string QualityFertilizer = "369";
        private const string DeluxeFertilizer = "919";

        public static bool ApplyEffect(GameLocation location, Vector2 targetPosition, HoeDirt targetDirt,
            AllelopathicInteraction interaction, float effectStrengthMultiplier)
        {
            if (targetDirt.crop == null)
                return false;

            float strength = interaction.EffectStrength * effectStrengthMultiplier;

            return (interaction.EffectType, interaction.CropEffect) switch
            {
                (AllelopathicEffectType.Positive, CropEffectType.GrowthSpeed) => ApplyGrowthSpeedBoost(targetDirt, strength),
                (AllelopathicEffectType.Negative, CropEffectType.GrowthSpeed) => ApplyGrowthSpeedPenalty(targetDirt, strength),
                (AllelopathicEffectType.Positive, CropEffectType.Quality) => ApplyQualityBoost(targetDirt, strength),
                (AllelopathicEffectType.Negative, CropEffectType.Quality) => ApplyQualityPenalty(targetDirt, strength),
                (AllelopathicEffectType.Positive, CropEffectType.Quantity) => ApplyQuantityBoost(targetDirt, strength),
                (AllelopathicEffectType.Negative, CropEffectType.Quantity) => ApplyQuantityPenalty(targetDirt, strength),
                (AllelopathicEffectType.Positive, _) => ApplyDefaultPositive(targetDirt, strength),
                (AllelopathicEffectType.Negative, _) => ApplyDefaultNegative(targetDirt, strength),
                _ => false
            };
        }

        /// <summary>GrowthSpeed+: Chance to advance growth phase.</summary>
        private static bool ApplyGrowthSpeedBoost(HoeDirt dirt, float strength)
        {
            if (dirt.crop == null) return false;

            if (Game1.random.NextDouble() < strength * 0.2 &&
                dirt.crop.currentPhase.Value < dirt.crop.phaseDays.Count - 1)
            {
                dirt.crop.currentPhase.Value++;
                dirt.crop.dayOfCurrentPhase.Value = 0;
                return true;
            }
            return false;
        }

        /// <summary>GrowthSpeed-: Chance to reset day progress in current phase.</summary>
        private static bool ApplyGrowthSpeedPenalty(HoeDirt dirt, float strength)
        {
            if (dirt.crop == null) return false;

            if (Game1.random.NextDouble() < strength * 0.2 && dirt.crop.dayOfCurrentPhase.Value > 0)
            {
                dirt.crop.dayOfCurrentPhase.Value = Math.Max(0, dirt.crop.dayOfCurrentPhase.Value - 1);
                return true;
            }
            return false;
        }

        /// <summary>Quality+: Chance to apply or upgrade fertilizer.</summary>
        private static bool ApplyQualityBoost(HoeDirt dirt, float strength)
        {
            if (Game1.random.NextDouble() >= strength * 0.15)
                return false;

            string current = dirt.fertilizer.Value;

            if (string.IsNullOrEmpty(current))
            {
                dirt.fertilizer.Value = BasicFertilizer;
                return true;
            }
            if (current == BasicFertilizer)
            {
                dirt.fertilizer.Value = QualityFertilizer;
                return true;
            }
            if (current == QualityFertilizer)
            {
                dirt.fertilizer.Value = DeluxeFertilizer;
                return true;
            }

            return false;
        }

        /// <summary>Quality-: Chance to downgrade or remove fertilizer.</summary>
        private static bool ApplyQualityPenalty(HoeDirt dirt, float strength)
        {
            if (Game1.random.NextDouble() >= strength * 0.15)
                return false;

            string current = dirt.fertilizer.Value;

            if (current == DeluxeFertilizer)
            {
                dirt.fertilizer.Value = QualityFertilizer;
                return true;
            }
            if (current == QualityFertilizer)
            {
                dirt.fertilizer.Value = BasicFertilizer;
                return true;
            }
            if (current == BasicFertilizer)
            {
                dirt.fertilizer.Value = null;
                return true;
            }

            return false;
        }

        /// <summary>Quantity+: Chance to advance growth (more harvest cycles for regrowth crops).</summary>
        private static bool ApplyQuantityBoost(HoeDirt dirt, float strength)
        {
            if (dirt.crop == null) return false;

            if (Game1.random.NextDouble() < strength * 0.15 &&
                dirt.crop.currentPhase.Value < dirt.crop.phaseDays.Count - 1)
            {
                dirt.crop.currentPhase.Value++;
                dirt.crop.dayOfCurrentPhase.Value = 0;
                return true;
            }
            return false;
        }

        /// <summary>Quantity-: Chance to delay growth (fewer harvest cycles).</summary>
        private static bool ApplyQuantityPenalty(HoeDirt dirt, float strength)
        {
            if (dirt.crop == null) return false;

            if (Game1.random.NextDouble() < strength * 0.2)
            {
                dirt.crop.dayOfCurrentPhase.Value = 0;
                return true;
            }
            return false;
        }

        /// <summary>Default+: Chance to auto-water dry soil.</summary>
        private static bool ApplyDefaultPositive(HoeDirt dirt, float strength)
        {
            if (dirt.state.Value == 0 && Game1.random.NextDouble() < strength * 0.25)
            {
                dirt.state.Value = 1;
                return true;
            }
            return false;
        }

        /// <summary>Default-: Chance to dry watered soil.</summary>
        private static bool ApplyDefaultNegative(HoeDirt dirt, float strength)
        {
            if (dirt.state.Value == 1 && Game1.random.NextDouble() < strength * 0.25)
            {
                dirt.state.Value = 0;
                return true;
            }
            return false;
        }

        public static Dictionary<Vector2, HoeDirt> GetCropsInRadius(GameLocation location, Vector2 centerPosition, int radius)
        {
            var cropsInRadius = new Dictionary<Vector2, HoeDirt>();

            for (int x = -radius; x <= radius; x++)
            {
                for (int y = -radius; y <= radius; y++)
                {
                    if (x == 0 && y == 0)
                        continue;

                    if (Math.Sqrt(x * x + y * y) <= radius)
                    {
                        Vector2 checkPosition = new Vector2(centerPosition.X + x, centerPosition.Y + y);

                        if (location.terrainFeatures.TryGetValue(checkPosition, out TerrainFeature feature) &&
                            feature is HoeDirt dirt &&
                            dirt.crop != null)
                        {
                            cropsInRadius.Add(checkPosition, dirt);
                        }
                    }
                }
            }

            return cropsInRadius;
        }

        public static int GetCropIdFromHoeDirt(HoeDirt dirt)
        {
            if (dirt?.crop == null)
                return -1;

            // In SDV 1.6, indexOfHarvest is a NetString containing the item ID (e.g. "190" for Cauliflower)
            string? harvestId = dirt.crop.indexOfHarvest.Value;
            if (!string.IsNullOrEmpty(harvestId) && int.TryParse(harvestId, out int result))
                return result;

            return -1;
        }
    }
}
