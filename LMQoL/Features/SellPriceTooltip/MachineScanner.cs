using System;
using System.Collections.Generic;
using StardewValley;
using StardewValley.GameData;
using StardewValley.GameData.Machines;
using Object = StardewValley.Object;

namespace LMQoL.Features.SellPriceTooltip
{
    /// <summary>Finds processing options by reading the game's own <c>Data/Machines</c>.
    ///
    /// This is what picks up modded machines — Cornucopia, Wildflour's Atelier Goods and anything
    /// else that registers machines the normal way — without hardcoding their recipes, so the list
    /// stays right when those mods add or rebalance products.
    ///
    /// Only rules whose output is a concrete item can be priced. Vanilla's flavoured products
    /// (wine, juice, jelly, pickles) are produced through a FLAVORED_ITEM token whose price comes
    /// from game code rather than data, so those stay with the hand-written rules in
    /// <see cref="PriceCalculator"/>.</summary>
    internal static class MachineScanner
    {
        private static readonly Dictionary<string, List<ProcessingOption>> Cache = new();

        /// <summary>Machine id prefixes belonging to mods we expose a separate toggle for.</summary>
        private const string CornucopiaPrefix = "(BC)Cornucopia";
        private const string WildflourPrefix = "(BC)Wildflour.AtelierGoods";

        /// <summary>Which of the recognised mods are actually installed; set at startup.</summary>
        internal static bool CornucopiaLoaded { get; set; }
        internal static bool WildflourLoaded { get; set; }

        public static void ClearCache() => Cache.Clear();

        /// <summary>Whether this machine's products should be listed.</summary>
        private static bool IsAllowed(string machineId)
        {
            if (machineId.StartsWith(CornucopiaPrefix, StringComparison.OrdinalIgnoreCase))
                return CornucopiaLoaded && ModEntry.Config.SellPriceIncludeCornucopia;

            if (machineId.StartsWith(WildflourPrefix, StringComparison.OrdinalIgnoreCase))
                return WildflourLoaded && ModEntry.Config.SellPriceIncludeWildflour;

            return true;   // vanilla and any other mod's machines
        }

        public static List<ProcessingOption> Scan(Object input)
        {
            // Quality is part of the key: with CopyQuality the same item at silver and at
            // iridium yields different prices, so one cached answer can't serve both.
            string key = $"{input.QualifiedItemId}:{input.Quality}";
            if (Cache.TryGetValue(key, out var cached))
                return cached;

            var results = new List<ProcessingOption>();
            try
            {
                foreach (var (machineId, machine) in DataLoader.Machines(Game1.content))
                {
                    if (machine?.OutputRules == null || !IsAllowed(machineId))
                        continue;

                    string? machineName = null;
                    foreach (var rule in machine.OutputRules)
                    {
                        if (rule?.Triggers == null || rule.OutputItem == null)
                            continue;

                        int inputCount = MatchedInputCount(rule.Triggers, input);
                        if (inputCount <= 0)
                            continue;

                        foreach (var output in rule.OutputItem)
                        {
                            int? price = PriceOf(output, input);
                            if (price is not > 0)
                                continue;

                            // Machines like the Dehydrator and Alembic eat several items per
                            // batch, so the batch price is divided by how many go in. Otherwise a
                            // 5-fruit product looks five times better than it is next to a
                            // one-fruit keg.
                            int perItem = price.Value / inputCount;
                            if (perItem <= 0)
                                continue;

                            machineName ??= DisplayName(machineId);
                            string productName = DisplayName(output.ItemId);
                            results.Add(new ProcessingOption(machineName, productName, perItem, inputCount));
                        }
                    }
                }
            }
            catch (Exception)
            {
                // a malformed machine entry from some mod shouldn't take the tooltip down
            }

            Cache[key] = results;
            return results;
        }

        /// <summary>How many of <paramref name="input"/> the first matching trigger consumes,
        /// or 0 when this rule doesn't accept the item at all.</summary>
        private static int MatchedInputCount(List<MachineOutputTriggerRule> triggers, Object input)
        {
            foreach (var trigger in triggers)
            {
                if (trigger == null || !trigger.Trigger.HasFlag(MachineOutputTrigger.ItemPlacedInMachine))
                    continue;

                bool matches;
                if (!string.IsNullOrEmpty(trigger.RequiredItemId))
                {
                    matches = trigger.RequiredItemId == input.QualifiedItemId || trigger.RequiredItemId == input.ItemId;
                }
                else if (trigger.RequiredTags is { Count: > 0 })
                {
                    matches = true;
                    foreach (string tag in trigger.RequiredTags)
                    {
                        if (!input.HasContextTag(tag))
                        {
                            matches = false;
                            break;
                        }
                    }
                }
                else
                {
                    continue;
                }

                if (matches)
                    return Math.Max(1, trigger.RequiredCount);
            }

            return 0;
        }

        /// <summary>Sell price of what the rule produces, or null when it can't be worked out
        /// statically (custom output methods, or item-id tokens resolved at runtime).</summary>
        private static int? PriceOf(MachineItemOutput output, Object input)
        {
            if (output == null || !string.IsNullOrEmpty(output.OutputMethod))
                return null;

            string? id = output.ItemId;
            if (string.IsNullOrEmpty(id) || id.Contains(' '))
                return null;   // token like "FLAVORED_ITEM Wine DROP_IN_ID"

            if (ItemRegistry.Create(id, 1, 0, allowNull: true) is not Object product)
                return null;

            // Cooked dishes aren't a way of "processing" the ingredient you're hovering — they're
            // recipes with several inputs — so they'd only crowd out the options that are.
            if (!ModEntry.Config.SellPriceIncludeFood && IsPreparedFood(product))
                return null;

            // CopyQuality passes the input's quality to the product, and quality is worth
            // +25% per star in sellToStorePrice — so ignoring it undersold everything made from
            // silver/gold/iridium ingredients.
            if (output.CopyQuality)
                product.Quality = input.Quality;

            if (output.QualityModifiers is { Count: > 0 })
            {
                float quality = product.Quality;
                foreach (var modifier in output.QualityModifiers)
                {
                    if (modifier != null && string.IsNullOrEmpty(modifier.Condition))
                        quality = QuantityModifier.Apply(quality, modifier.Modification, modifier.Amount);
                }
                product.Quality = Math.Clamp((int)quality, 0, 4);
            }

            // CopyPrice means the product inherits the input's value, which the rule then scales.
            if (output.CopyPrice)
            {
                float basePrice = input.Price;
                if (output.PriceModifiers is { Count: > 0 })
                {
                    foreach (var modifier in output.PriceModifiers)
                    {
                        if (modifier != null && string.IsNullOrEmpty(modifier.Condition))
                            basePrice = QuantityModifier.Apply(basePrice, modifier.Modification, modifier.Amount);
                    }
                }
                product.Price = (int)basePrice;
            }

            // Price it as the finished product: profession bonuses key off the PRODUCT's category
            // (Artisan on artisan goods, Tapper on syrup, and so on), not the input's.
            int price = PriceCalculator.SellPrice(product);

            // a rule that yields several of the product is worth that much more
            int stack = Math.Max(1, output.MinStack > 0 ? output.MinStack : 1);
            return price * stack;
        }

        /// <summary>Prepared food: category Cooking, or the Cooking object type for the odd
        /// modded dish that doesn't set the category.</summary>
        private static bool IsPreparedFood(Object product)
            => product.Category == Object.CookingCategory
               || string.Equals(product.Type, "Cooking", StringComparison.OrdinalIgnoreCase);

        private static string DisplayName(string? itemId)
        {
            if (string.IsNullOrEmpty(itemId))
                return "?";
            return ItemRegistry.GetData(itemId)?.DisplayName ?? itemId;
        }
    }
}
