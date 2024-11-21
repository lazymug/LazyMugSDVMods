using StardewValley;
using StardewValley.GameData.BigCraftables;

namespace RuralMagazineCompetition.Models.Reward
{
    public class BigCraftableReward : IReward
    {
        public BigCraftableData BigCraftable { get; set; }

        public void GrantReward(Farmer farmer)
        {
            // Add the BigCraftable to the farm
            // ...
        }
    }
}