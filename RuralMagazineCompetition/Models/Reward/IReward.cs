using StardewValley;

namespace RuralMagazineCompetition.Models.Reward
{
    public interface IReward
    {
        void GrantReward(Farmer farmer);
    }
}