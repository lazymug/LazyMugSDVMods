using System.Linq;
using System.Text;
using HarmonyLib;

namespace LMQoL.Features.SellPriceTooltip
{
    [HarmonyPatch(typeof(StardewValley.Object), nameof(StardewValley.Object.getDescription))]
    public static class TooltipPatcher
    {
        public static void Postfix(StardewValley.Object __instance, ref string __result)
        {
            var config = ModEntry.Config;
            if (!config.SellPriceTooltipEnabled)
                return;

            var options = PriceCalculator.GetOptions(__instance);
            if (options.Count == 0)
                return;

            bool highlight = config.SellPriceHighlightBest;

            // Prices already include the player's profession bonuses (see PriceCalculator), so
            // they're compared and displayed as-is.
            int bestPrice = options.Max(o => o.Price);

            // most valuable first; modded machine sets can accept the same item many times over,
            // so cap the list rather than letting the tooltip run off the screen
            int limit = System.Math.Max(1, config.SellPriceMaxOptions);
            var shown = options.OrderByDescending(o => o.Price).ThenBy(o => o.MachineName).Take(limit).ToList();
            int hidden = options.Count - shown.Count;

            var sb = new StringBuilder(__result);
            sb.Append("\n\n--- Processing ---");

            foreach (var option in shown)
            {
                string marker = highlight && option.Price == bestPrice && options.Count > 1 ? " *" : "";
                // make it obvious the figure is per item when a batch eats several
                string batch = option.InputCount > 1 ? $" /{option.InputCount}x" : "";

                sb.Append($"\n{option.MachineName}: {option.Price}g ({option.ProductName}{batch}){marker}");
            }

            if (hidden > 0)
                sb.Append($"\n(+{hidden} more)");

            if (highlight && options.Count > 1)
                sb.Append("\n* = best profit");

            __result = sb.ToString();
        }
    }
}
