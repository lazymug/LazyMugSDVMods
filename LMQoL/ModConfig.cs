using StardewModdingAPI.Utilities;

namespace LMQoL
{
    public class ModConfig
    {
        // Auto Gate
        public bool AutoGateEnabled { get; set; } = true;
        public int AutoGateCloseDelayTicks { get; set; } = 60;

        // Magnet Radius Foraging
        public bool MagnetForagingEnabled { get; set; } = true;
        public int MagnetForagingRadius { get; set; } = 5;
        public int MagnetForagingSpeed { get; set; } = 8;

        // Sell Price Tooltip
        public bool SellPriceTooltipEnabled { get; set; } = true;
        public bool SellPriceShowArtisan { get; set; } = true;
        public bool SellPriceHighlightBest { get; set; } = true;

        // Quick Stack to Nearby Chests
        public bool QuickStackEnabled { get; set; } = true;
        public int QuickStackRadius { get; set; } = 5;
        public KeybindList QuickStackKey { get; set; } = KeybindList.Parse("Z");

        // Silo Capacity
        public bool SiloCapacityEnabled { get; set; } = false;
        public int SiloCapacity { get; set; } = 240;

        // Species Tooltip (master switch + per-kind toggles)
        public bool SpeciesTooltipEnabled { get; set; } = true;
        public bool SpeciesTooltipCrops { get; set; } = true;
        public bool SpeciesTooltipTrees { get; set; } = true;
        public bool SpeciesTooltipBushes { get; set; } = true;

        // Sell Anything
        public bool SellAnythingEnabled { get; set; } = true;
        public bool SellAnythingShipping { get; set; } = true;
        public bool SellAnythingShops { get; set; } = false;

        // Custom Charcoal Kiln
        public bool CharcoalKilnEnabled { get; set; } = true;

        // Build With Bags (only does anything when Item Bags is installed)
        public bool BuildWithBagsEnabled { get; set; } = true;

        // Item Totals tooltip inside an open Item Bag
        public bool ItemTotalsEnabled { get; set; } = true;
        public bool ItemTotalsIncludeChests { get; set; } = true;
        public bool ItemTotalsIncludeBags { get; set; } = true;
    }
}
