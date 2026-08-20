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

            bool hasArtisan = config.SellPriceShowArtisan && PriceCalculator.PlayerHasArtisan();
            bool highlight = config.SellPriceHighlightBest;

            // the price as it will actually be shown, so the ordering and the "best" marker agree
            int Effective(ProcessingOption o) => hasArtisan ? PriceCalculator.ApplyArtisan(o.Price) : o.Price;

            int bestPrice = options.Max(Effective);

            // most valuable first; modded machine sets can accept the same item many times over,
            // so cap the list rather than letting the tooltip run off the screen
            int limit = System.Math.Max(1, config.SellPriceMaxOptions);
            var shown = options.OrderByDescending(Effective).ThenBy(o => o.MachineName).Take(limit).ToList();
            int hidden = options.Count - shown.Count;

            var sb = new StringBuilder(__result);
            sb.Append("\n\n--- Processing ---");

            foreach (var option in shown)
            {
                int price = Effective(option);
                string marker = highlight && price == bestPrice && options.Count > 1 ? " *" : "";
                string artisanTag = hasArtisan ? " [A]" : "";

                sb.Append($"\n{option.MachineName}: {price}g ({option.ProductName}){artisanTag}{marker}");
            }

            if (hidden > 0)
                sb.Append($"\n(+{hidden} more)");

            if (highlight && options.Count > 1)
                sb.Append("\n* = best profit");

            __result = sb.ToString();
        }
    }
}
