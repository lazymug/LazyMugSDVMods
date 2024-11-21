using StardewValley;

namespace RuralMagazineCompetition.Models.Reward
{
    public class ItemReward : IReward
    {
        public Item Item { get; set; }

        public void GrantReward(Farmer farmer)
        {
            farmer.addItemToInventory(Item);
        }
    }
}