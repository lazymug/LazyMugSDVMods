using RuralMagazineCompetition.Models.Reward;

namespace RuralMagazineCompetition.Models
{
    public class PrizeTier
    {
        public int MinPoints { get; set; }
        public int MaxPoints { get; set; }
        public IReward Prize { get; set; }
    }
}