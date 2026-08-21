namespace LMAutomateCrops
{
    public class ModConfig
    {
        /// <summary>Whether Automate harvests crops at all.</summary>
        public bool Enabled { get; set; } = true;

        /// <summary>Grant the same Farming experience a manual harvest would.</summary>
        public bool GrantExperience { get; set; } = true;

        /// <summary>Replant one-off crops from seeds in the connected chests.</summary>
        public bool Replant { get; set; } = true;

        /// <summary>Automate crops in the greenhouse.</summary>
        public bool IncludeGreenhouse { get; set; } = true;

        /// <summary>Automate crops on Ginger Island.</summary>
        public bool IncludeGingerIsland { get; set; } = true;
    }
}
