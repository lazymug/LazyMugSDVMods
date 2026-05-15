using System.Collections.Generic;
using StardewValley;

namespace FriendlyTrades.Models
{
    public class NpcSpecialty
    {
        public string NpcName { get; set; } = "";
        public string SpecialtyKey { get; set; } = "";
        public string ShopId { get; set; } = "";
        public string ShopLocationName { get; set; } = "";
        public HashSet<int> AcceptedCategories { get; set; } = new();
        public HashSet<string> AcceptedItemIds { get; set; } = new();

        /// <summary>
        /// Categories that are only accepted when item quality meets the minimum.
        /// Key = category, Value = minimum quality (0=Normal, 1=Silver, 2=Gold, 4=Iridium).
        /// </summary>
        public Dictionary<int, int> QualityMinCategories { get; set; } = new();

        public bool AcceptsItem(ISalable salable)
        {
            if (salable is not StardewValley.Object obj)
                return false;

            if (AcceptedCategories.Contains(obj.Category))
                return true;

            if (QualityMinCategories.TryGetValue(obj.Category, out int minQuality) && obj.Quality >= minQuality)
                return true;

            if (AcceptedItemIds.Contains(obj.ItemId))
                return true;

            return false;
        }
    }
}
