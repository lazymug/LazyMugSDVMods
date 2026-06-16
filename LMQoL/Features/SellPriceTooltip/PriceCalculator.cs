using System.Collections.Generic;
using StardewValley;

namespace LMQoL.Features.SellPriceTooltip
{
    public record ProcessingOption(string MachineName, string ProductName, int Price);

    public static class PriceCalculator
    {
        private const int CategoryFruit = -79;
        private const int CategoryVegetable = -75;
        private const int CategoryFish = -4;

        public static List<ProcessingOption> GetOptions(StardewValley.Object item)
        {
            var options = new List<ProcessingOption>();
            int basePrice = item.Price;
            string itemId = item.ItemId;
            int category = item.Category;

            var kegOption = GetKegOption(itemId, category, basePrice);
            if (kegOption != null)
                options.Add(kegOption);

            var jarOption = GetJarOption(category, basePrice);
            if (jarOption != null)
                options.Add(jarOption);

            var oilOption = GetOilMakerOption(itemId);
            if (oilOption != null)
                options.Add(oilOption);

            var smokerOption = GetSmokerOption(category, basePrice);
            if (smokerOption != null)
                options.Add(smokerOption);

            return options;
        }

        public static int ApplyArtisan(int price)
        {
            return (int)(price * 1.4);
        }

        public static bool PlayerHasArtisan()
        {
            return Game1.player.professions.Contains(Farmer.artisan);
        }

        private static ProcessingOption? GetKegOption(string itemId, int category, int basePrice)
        {
            return itemId switch
            {
                "262" => new ProcessingOption("Keg", "Beer", 200),
                "304" => new ProcessingOption("Keg", "Pale Ale", 300),
                "433" => new ProcessingOption("Keg", "Coffee", 150),
                "340" => new ProcessingOption("Keg", "Mead", 200),
                "815" => new ProcessingOption("Keg", "Green Tea", 100),
                _ => category switch
                {
                    CategoryFruit => new ProcessingOption("Keg", "Wine", basePrice * 3),
                    CategoryVegetable => new ProcessingOption("Keg", "Juice", (int)(basePrice * 2.25)),
                    _ => null
                }
            };
        }

        private static ProcessingOption? GetJarOption(int category, int basePrice)
        {
            return category switch
            {
                CategoryFruit => new ProcessingOption("Jar", "Jelly", basePrice * 2 + 50),
                CategoryVegetable => new ProcessingOption("Jar", "Pickles", basePrice * 2 + 50),
                _ => null
            };
        }

        private static ProcessingOption? GetOilMakerOption(string itemId)
        {
            return itemId switch
            {
                "430" => new ProcessingOption("Oil Maker", "Truffle Oil", 1065),
                "270" => new ProcessingOption("Oil Maker", "Oil", 100),
                "421" => new ProcessingOption("Oil Maker", "Oil", 100),
                "431" => new ProcessingOption("Oil Maker", "Oil", 100),
                _ => null
            };
        }

        private static ProcessingOption? GetSmokerOption(int category, int basePrice)
        {
            if (category == CategoryFish)
                return new ProcessingOption("Smoker", "Smoked Fish", basePrice * 2);
            return null;
        }
    }
}
