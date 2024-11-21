using RuralMagazineCompetition.Enums;
using RuralMagazineCompetition.Models;
using RuralMagazineCompetition.Models.Reward;
using StardewValley;

namespace RuralMagazineCompetition.Helpers
{
    public interface IMailHelper
    {
        public void Initialize(string gameUniqueId);
        
        public void SendMailOfCompetitionPresentation(Farmer farmer);

        public void SendMailWithItemsToBeValuatedOnNextSeason(Dictionary<string, ItemInfo> items);

        public void SendMailWithPrize(CompetitionCategoryEnum category, int position, IReward reward);
    }
}