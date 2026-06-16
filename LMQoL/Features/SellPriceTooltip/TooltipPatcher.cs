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

            int bestPrice = options.Max(o => hasArtisan ? PriceCalculator.ApplyArtisan(o.Price) : o.Price);

            var sb = new StringBuilder(__result);
            sb.Append("\n\n--- Processing ---");

            foreach (var option in options)
            {
                int price = hasArtisan ? PriceCalculator.ApplyArtisan(option.Price) : option.Price;
                string marker = highlight && price == bestPrice && options.Count > 1 ? " *" : "";
                string artisanTag = hasArtisan ? " [A]" : "";

                sb.Append($"\n{option.MachineName}: {price}g ({option.ProductName}){artisanTag}{marker}");
            }

            if (highlight && options.Count > 1)
                sb.Append("\n* = best profit");

            __result = sb.ToString();
        }
    }
}
