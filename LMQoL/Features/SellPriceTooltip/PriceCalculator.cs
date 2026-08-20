using System.Collections.Generic;
using StardewValley;
using StardewValley.ItemTypeDefinitions;
using Object = StardewValley.Object;

namespace LMQoL.Features.SellPriceTooltip
{
    /// <param name="Price">Value per input item, so options that consume several items per batch
/// stay comparable with ones that take a single item.</param>
/// <param name="InputCount">How many input items one batch consumes.</param>
public record ProcessingOption(string MachineName, string ProductName, int Price, int InputCount = 1);

    public static class PriceCalculator
    {
        private const int CategoryFruit = -79;
        private const int CategoryVegetable = -75;
        private const int CategoryFish = -4;

        public static List<ProcessingOption> GetOptions(Object item)
        {
            var options = new List<ProcessingOption>();

            // Flavoured products can't be priced from Data/Machines: the recipe there is a
            // FLAVORED_ITEM token and the real item is built in code. Build the actual product so
            // the price includes the same multipliers the shop would apply.
            var objects = ItemRegistry.GetObjectTypeDefinition();

            switch (item.Category)
            {
                case CategoryFruit:
                    Add(options, "Keg", objects.CreateFlavoredWine(item));
                    Add(options, "Preserves Jar", objects.CreateFlavoredJelly(item));
                    break;

                case CategoryVegetable:
                    Add(options, "Keg", objects.CreateFlavoredJuice(item));
                    Add(options, "Preserves Jar", objects.CreateFlavoredPickle(item));
                    break;

                case CategoryFish:
                    Add(options, "Smoker", objects.CreateFlavoredSmokedFish(item));
                    break;
            }

            // Everything registered in Data/Machines — vanilla and modded alike.
            if (ModEntry.Config.SellPriceScanMachines)
            {
                var seen = new HashSet<string>();
                foreach (var option in options)
                    seen.Add(option.ProductName);

                foreach (var option in MachineScanner.Scan(item))
                {
                    if (seen.Add(option.ProductName))
                        options.Add(option);
                }
            }

            return options;
        }

        private static void Add(List<ProcessingOption> options, string machineName, Object? product)
        {
            if (product == null)
                return;

            int price = SellPrice(product);
            if (price > 0)
                options.Add(new ProcessingOption(machineName, product.DisplayName, price));
        }

        /// <summary>What the product is actually worth to the player.
        ///
        /// <c>sellToStorePrice()</c> is the game's own calculation, so it accounts for every
        /// profession bonus that applies to that product's category — Artisan on artisan goods,
        /// Tiller on crops, Rancher on animal products, Fisher/Angler on fish, Tapper on syrups,
        /// Blacksmith on bars, Gemologist on gems — plus quality and the difficulty modifier.
        /// Turning the option off shows the plain base price instead.</summary>
        public static int SellPrice(Object product)
        {
            return ModEntry.Config.SellPriceApplyProfessions
                ? product.sellToStorePrice()
                : product.Price;
        }
    }
}
