using Object = StardewValley.Object;

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

        /// <summary>Harvest crops whose produce is a vegetable.</summary>
        public bool HarvestVegetables { get; set; } = true;

        /// <summary>Harvest crops whose produce is a fruit.</summary>
        public bool HarvestFruit { get; set; } = true;

        /// <summary>Harvest crops whose produce is a flower.</summary>
        public bool HarvestFlowers { get; set; } = true;

        /// <summary>Harvest crops whose produce is forage — Fiber, Cotton Boll and the like.</summary>
        public bool HarvestForage { get; set; } = true;

        /// <summary>Harvest crops whose produce is itself a seed, such as Sesame Seeds.</summary>
        public bool HarvestSeeds { get; set; } = true;

        /// <summary>Harvest crops whose produce falls into none of the categories above.</summary>
        public bool HarvestOther { get; set; } = true;

        /// <summary>Whether produce of this item category should be picked.</summary>
        public bool IsHarvested(int category) => category switch
        {
            Object.VegetableCategory => HarvestVegetables,
            Object.FruitsCategory => HarvestFruit,
            Object.flowersCategory => HarvestFlowers,
            Object.GreensCategory => HarvestForage,
            Object.SeedsCategory => HarvestSeeds,
            _ => HarvestOther,
        };
    }
}
