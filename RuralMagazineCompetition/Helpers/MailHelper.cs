using MailFrameworkMod;
using RuralMagazineCompetition.Enums;
using RuralMagazineCompetition.Models;
using RuralMagazineCompetition.Models.Reward;
using StardewModdingAPI;
using StardewValley;

namespace RuralMagazineCompetition.Helpers
{
    public class MailHelper : IMailHelper
    {
        private string GameUniqueId;

        public void Initialize(string gameUniqueId)
        {
            GameUniqueId = gameUniqueId;
        }

        public void SendMailOfCompetitionPresentation(Farmer farmer)
        {
            // todo: check break line
            var mailId = GetIdPrefix() + ".Presentation";
            MailRepository.SaveLetter(new Letter(
                mailId,
                string.Format(ModEntry.Instance.Helper.Translation.Get("rural.magazine.competition.welcome.mail"), farmer.Name),
                // GetMailListOfItems(name), // Send Money
                (l)=>!farmer.mailReceived.Contains(l.Id) && l.Id.Contains(GetIdPrefix()),
                (l)=>farmer.mailReceived.Add(l.Id)
            ));
        }
        
        public void SendMailWithItemsToBeValuatedOnNextSeason(Dictionary<string, ItemInfo> items)
        {
            
        }

        public void SendMailWithPrize(CompetitionCategoryEnum category, int position, IReward? reward)
        {
            
        }

        private string GetIdPrefix()
        {
            return $"{GameUniqueId}.RuralMagazineCompetition";
        }
    }
}