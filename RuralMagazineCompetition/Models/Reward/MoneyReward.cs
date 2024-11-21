using StardewValley;

namespace RuralMagazineCompetition.Models.Reward
{
    public class MoneyReward : IReward
    {
        public int Amount { get; set; }

        public void GrantReward(Farmer farmer)
        {
            farmer.Money += Amount;
        }
    }
}