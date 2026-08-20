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
            string key = input.QualifiedItemId;
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
                        if (rule?.Triggers == null || rule.OutputItem == null || !MatchesAny(rule.Triggers, input))
                            continue;

                        foreach (var output in rule.OutputItem)
                        {
                            int? price = PriceOf(output, input);
                            if (price is not > 0)
                                continue;

                            machineName ??= DisplayName(machineId);
                            string productName = DisplayName(output.ItemId);
                            results.Add(new ProcessingOption(machineName, productName, price.Value));
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

        private static bool MatchesAny(List<MachineOutputTriggerRule> triggers, Object input)
        {
            foreach (var trigger in triggers)
            {
                if (trigger == null || !trigger.Trigger.HasFlag(MachineOutputTrigger.ItemPlacedInMachine))
                    continue;

                if (!string.IsNullOrEmpty(trigger.RequiredItemId))
                {
                    if (trigger.RequiredItemId == input.QualifiedItemId || trigger.RequiredItemId == input.ItemId)
                        return true;
                    continue;
                }

                if (trigger.RequiredTags is { Count: > 0 })
                {
                    bool all = true;
                    foreach (string tag in trigger.RequiredTags)
                    {
                        if (!input.HasContextTag(tag))
                        {
                            all = false;
                            break;
                        }
                    }
                    if (all)
                        return true;
                }
            }

            return false;
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

            float price;
            if (output.CopyPrice)
            {
                price = input.Price;
            }
            else
            {
                var product = ItemRegistry.Create(id, 1, 0, allowNull: true);
                if (product == null)
                    return null;
                price = product.sellToStorePrice();
            }

            if (output.PriceModifiers is { Count: > 0 })
            {
                foreach (var modifier in output.PriceModifiers)
                {
                    if (modifier != null && string.IsNullOrEmpty(modifier.Condition))
                        price = QuantityModifier.Apply(price, modifier.Modification, modifier.Amount);
                }
            }

            // a rule that yields several of the product is worth that much more
            int stack = Math.Max(1, output.MinStack > 0 ? output.MinStack : 1);
            return (int)(price * stack);
        }

        private static string DisplayName(string? itemId)
        {
            if (string.IsNullOrEmpty(itemId))
                return "?";
            return ItemRegistry.GetData(itemId)?.DisplayName ?? itemId;
        }
    }
}
