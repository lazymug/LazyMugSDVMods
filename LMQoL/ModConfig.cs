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
    }
}
