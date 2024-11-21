using RuralMagazineCompetition.Enums;
using StardewValley;

namespace RuralMagazineCompetition.Models
{
    public class ItemInfo
    {
        public int BasePoints { get; set; }
        public CompetitionCategoryEnum Category { get; set; }
        public List<Season> AvailableSeasons { get; set; }
    }
}