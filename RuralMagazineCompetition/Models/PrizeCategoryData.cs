using RuralMagazineCompetition.Enums;

namespace RuralMagazineCompetition.Models
{
    public class PrizeCategoryData
    {
        public CompetitionCategoryEnum Category { get; set; }
        public List<PrizeTier> PrizeTiers { get; set; }
    }
}