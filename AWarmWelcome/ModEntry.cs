using AWarmWelcome.Utils;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using MailFrameworkMod;
using StardewValley;

namespace AWarmWelcome
{
    internal sealed class ModEntry : Mod
    {
        private readonly string[] _villagersVanilla =
        {
            "Abigail", "Alex", "Elliot", "Emily", "Haley", "Harvey", "Leah", "Maru", "Penny", "Sam",
            "Sebastian", "Shane", "Caroline", "Clint", "Demetrius", "Dwarf", "Evelyn", "George", 
            "Gus", "Jas", "Jodi", "Kent", "Krobus", "Lewis", "Linus", "Marnie", "Pam", "Pierre",
            "Robin", "Sandy", "Vincent", "Willy"
        };
        
        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
        }
        
        private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
        {
            foreach (var npc in _villagersVanilla)
            {
                var stringKey = GetMailStringKey(npc);
                var letterId = $"{Game1.uniqueIDForThisGame}.{ModManifest.UniqueID}.{stringKey}";
                MailRepository.SaveLetter(new Letter(
                    letterId,
                    GetMailContent(stringKey),
                    GetMailListOfItems(npc),
                    l=>!Game1.player.mailReceived.Contains(l.Id) && l.Id.Contains(Game1.uniqueIDForThisGame.ToString()) && new Switch<String, bool>(npc)
                        .Case("Abigail").Then(Game1.player.friendshipData.ContainsKey(npc) && Game1.player.friendshipData[npc]?.Points >= 150)
                        .Case("Alex").Then(Game1.player.friendshipData.ContainsKey(npc) && Game1.player.friendshipData[npc]?.Points >= 150)
                        .Case("Elliot").Then(Game1.player.friendshipData.ContainsKey(npc) && Game1.player.friendshipData[npc]?.Points >= 150)
                        .Case("Emily").Then(Game1.player.friendshipData.ContainsKey(npc) && Game1.player.friendshipData[npc]?.Points >= 150)
                        .Case("Haley").Then(Game1.player.friendshipData.ContainsKey(npc) && Game1.player.friendshipData[npc]?.Points >= 150)
                        .Case("Harvey").Then(Game1.player.friendshipData.ContainsKey(npc) && Game1.player.friendshipData[npc]?.Points >= 150)
                        .Case("Leah").Then(Game1.player.friendshipData.ContainsKey(npc) && Game1.player.friendshipData[npc]?.Points >= 150)
                        .Case("Maru").Then(Game1.player.friendshipData.ContainsKey(npc) && Game1.player.friendshipData[npc]?.Points >= 150)
                        .Case("Penny").Then(Game1.player.friendshipData.ContainsKey(npc) && Game1.player.friendshipData[npc]?.Points >= 150)
                        .Case("Sam").Then(Game1.player.friendshipData.ContainsKey(npc) && Game1.player.friendshipData[npc]?.Points >= 150)
                        .Case("Sebastian").Then(Game1.player.friendshipData.ContainsKey(npc) && Game1.player.friendshipData[npc]?.Points >= 150)
                        .Case("Shane").Then(Game1.player.friendshipData.ContainsKey(npc) && Game1.player.friendshipData[npc]?.Points >= 150)
                        .Case("Lewis").Then(Game1.dayOfMonth > 1)
                        .Case("Robin").Then(Game1.dayOfMonth > 1)
                        .Case("Pierre").Then(Game1.dayOfMonth > 3)
                        .Case("Kent").Then(Game1.year > 1 && Game1.dayOfMonth > 6)
                        .Case("Dwarf").Then(Game1.player.canUnderstandDwarves)
                        .Case("Krobus").Then(Game1.player.hasRustyKey)
                        .Case("Caroline").Then(Game1.dayOfMonth > 5)
                        .Case("Demetrius").Then(Game1.dayOfMonth > 6)
                        .Case("Marnie").Then(Game1.dayOfMonth > 7)
                        .Case("Jodi").Then(Game1.dayOfMonth > 9)
                        .Case("Willy").Then(Game1.player.FishingLevel >= 1)
                        .Case("Clint").Then(Game1.player.eventsSeen.Contains("992553"))
                        .Case("Evelyn").Then(Game1.dayOfMonth > 4)
                        .Case("George").Then(Game1.dayOfMonth > 4)
                        .Case("Gus").Then(Game1.dayOfMonth > 2)
                        .Case("Jas").Then(Game1.dayOfMonth > 17)
                        .Case("Linus").Then(Game1.player.friendshipData.ContainsKey(npc) && Game1.player.friendshipData[npc]?.Points >= 200)
                        .Case("Pam").Then(Game1.dayOfMonth > 13)
                        .Case("Sandy").Then(Game1.season == Season.Summer && Game1.dayOfMonth > 2)
                        .Case("Vincent").Then(Game1.dayOfMonth > 15)
                        .Default(false),
                    (l)=>Game1.player.mailReceived.Add(l.Id),
                    whichBG: npc == "Sandy" ? 1 : 0
                ));
            }
        }

        private string GetMailContent(string mailId)
        {
            return Helper.Translation.Get(mailId);
        }

        private string GetMailStringKey(string npcName)
        {
            return $"welcome.farmer.{npcName}";
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