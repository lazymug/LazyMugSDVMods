using AWarmWelcome.Utils;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using MailFrameworkMod;
using StardewValley;

namespace AWarmWelcome
{
    internal sealed class ModEntry : Mod
    {
        
        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.DayEnding += OnDayEnding;
        }
        
        private void OnDayEnding(object? sender, DayEndingEventArgs e)
        {
            var sentPerDay = 0;
            foreach (var friend in Game1.player.friendshipData)
            {
                foreach (var name in friend.Keys)
                {
                    var friendship = friend[name];
                    if (friendship.Points < 150 || sentPerDay > 2) continue;
                    var mailId = GetMailId(name);
                    MailRepository.SaveLetter(new Letter(
                        $"{Game1.uniqueIDForThisGame}.{mailId}",
                        GetMailContent(mailId),
                        GetMailListOfItems(name),
                        (l)=>!Game1.player.mailReceived.Contains(l.Id) && l.Id.Contains(Game1.uniqueIDForThisGame.ToString()),
                        (l)=>Game1.player.mailReceived.Add(l.Id)
                    ));
                    sentPerDay++;
                }
            }
        }

        private string GetMailContent(string mailId)
        {
            return string.Format(Helper.Translation.Get(mailId), Game1.player.Name);
        }

        private string GetMailId(string npcName)
        {
            return string.Format($"welcome.farmer.{npcName}");
        }

        private List<Item> GetMailListOfItems(string npcName)
        {
            return new Switch<String, List<Item>>(npcName)
                .Case("Abigail").Then(new List<Item>() { new StardewValley.Object("66", 1) })
                .Case("Alex").Then(new List<Item>() { new StardewValley.Object("176", 3) })
                .Case("Elliot").Then(new List<Item>() { new StardewValley.Object("218", 1) })
                .Case("Emily").Then(new List<Item>() { new StardewValley.Object("440", 1) })
                .Case("Haley").Then(new List<Item>() { new StardewValley.Object("421", 1) })
                .Case("Harvey").Then(new List<Item>() { new StardewValley.Object("395", 1) })
                .Case("Leah").Then(new List<Item>() { new StardewValley.Object("196", 1) })
                .Case("Maru").Then(new List<Item>() { new StardewValley.Object("787", 1) })
                .Case("Penny").Then(new List<Item>() { new StardewValley.Object("376", 1) })
                .Case("Sam").Then(new List<Item>() { new StardewValley.Object("167", 1) })
                .Case("Sebastian").Then(new List<Item>() { new StardewValley.Object("84", 1) })
                .Case("Shane").Then(new List<Item>() { new StardewValley.Object("346", 1) })
                .Case("Caroline").Then(new List<Item>() { new StardewValley.Object("472", 3) })
                .Case("Clint").Then(new List<Item>() { new StardewValley.Object("378", 5) })
                .Case("Demetrius").Then(new List<Item>() { new StardewValley.Object("473", 1) })
                .Case("Dwarf").Then(new List<Item>() { new StardewValley.Object("64", 1) })
                .Case("Evelyn").Then(new List<Item>() { new StardewValley.Object("223", 1) })
                .Case("George").Then(new List<Item>() { new StardewValley.Object("20", 1) })
                .Case("Gus").Then(new List<Item>() { new StardewValley.Object("224", 1) })
                .Case("Jas").Then(new List<Item>() { new StardewValley.Object("221", 1) })
                .Case("Jodi").Then(new List<Item>() { new StardewValley.Object("211", 1) })
                .Case("Kent").Then(new List<Item>() { new StardewValley.Object("252", 1) })
                .Case("Krobus").Then(new List<Item>() { new StardewValley.Object("305", 1) })
                .Case("Lewis").Then(new List<Item>() { new StardewValley.Object("24", 1) })
                .Case("Linus").Then(new List<Item>() { new StardewValley.Object("16", 1) })
                .Case("Marnie").Then(new List<Item>() { new StardewValley.Object("178", 10) })
                .Case("Pam").Then(new List<Item>() { new StardewValley.Object("346", 1) })
                .Case("Pierre").Then(new List<Item>() { new StardewValley.Object("475", 3) })
                .Case("Robin").Then(new List<Item>() { new StardewValley.Object("388", 50) })
                .Case("Sandy").Then(new List<Item>() { new StardewValley.Object("90", 1) })
                .Case("Vincent").Then(new List<Item>() { new StardewValley.Object("398", 1) })
                .Case("Willy").Then(new List<Item>() { new StardewValley.Object("685", 10) })
                .Default(new List<Item>());
        }
    }
}