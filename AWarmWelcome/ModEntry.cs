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

        private readonly string[] _villagersExpanded =
        {
            "Andy", "Gunther", "Morris", "Olivia", "Sophia", "Victor"
        };

        // Name = NPC internal id (matches friendshipData key and the "welcome.farmer.<Name>" i18n key).
        // Romanceable villagers arrive at 2.5 hearts (150 pts); the rest arrive after the given day of month.
        private record WelcomeNpc(string Name, string ItemId, int Quantity, bool Romanceable, int Day);

        // Ridgeside Village (Rafseazz.RidgesideVillage). Gifts are custom RSV items the villager loves.
        private readonly WelcomeNpc[] _villagersRidgeside =
        {
            // Romanceable -> friendship gated
            new("Ysabelle", "Rafseazz.RSVCP_Golden_Rose_Fin", 1, true, 0),
            new("Flor", "Rafseazz.RSVCP_Sweet_Cranberry_Cheesecake", 1, true, 0),
            new("Philip", "Rafseazz.RSVCP_Jumpy_Coffee_Cake", 1, true, 0),
            new("Ian", "Rafseazz.RSVCP_Fried_Fish_a_la_Ridge", 1, true, 0),
            new("Sean", "Rafseazz.RSVCP_Highland_Butterwort", 1, true, 0),
            new("Kenneth", "Rafseazz.RSVCP_Mountain_Hokkaido", 1, true, 0),
            new("Maddie", "Rafseazz.RSVCP_Pink_Frosted_Sprinkled_Doughnut", 1, true, 0),
            new("Corine", "Rafseazz.RSVCP_Wild_Apple_Juice", 1, true, 0),
            new("Kiarra", "Rafseazz.RSVCP_Summer_Mountain_Blessing", 1, true, 0),
            new("Jeric", "Rafseazz.RSVCP_Winter_Night_Feast", 1, true, 0),
            new("Shiro", "Rafseazz.RSVCP_Summit_Snowbell", 1, true, 0),
            new("Blair", "Rafseazz.RSVCP_Fluffy_Apple_Crumble", 1, true, 0),
            new("Alissa", "Rafseazz.RSVCP_Aurorean_Iris", 1, true, 0),
            new("June", "Rafseazz.RSVCP_Autumnal_Serenity", 1, true, 0),
            // Day gated (3-4 per day)
            new("Aguar", "Rafseazz.RSVCP_Golden_Skull_Coral", 1, false, 2),
            new("Bert", "Rafseazz.RSVCP_Crunchy_Bagel", 1, false, 2),
            new("Ezekiel", "Rafseazz.RSVCP_Ridgeside_Monkshood", 1, false, 2),
            new("Freddie", "Rafseazz.RSVCP_Winter_Night_Feast", 1, false, 2),
            new("Keahi", "Rafseazz.RSVCP_Highland_Ice_Cream", 1, false, 4),
            new("Lenny", "Rafseazz.RSVCP_Highland_Revani", 1, false, 4),
            new("Lola", "Rafseazz.RSVCP_Lava_Lily", 1, false, 4),
            new("Olga", "Rafseazz.RSVCP_Strawberry_Lover_Pie", 1, false, 4),
            new("Pika", "Rafseazz.RSVCP_Lava_Lily", 1, false, 6),
            new("Richard", "Rafseazz.RSVCP_Honey_Ginger_Whitefish", 1, false, 6),
            new("Yuuma", "Rafseazz.RSVCP_Cherry_Berry_Shakey", 1, false, 6),
            new("Trinnie", "Rafseazz.RSVCP_Pink_Frosted_Sprinkled_Doughnut", 1, false, 6),
            new("Ariah", "Rafseazz.RSVCP_Mountain_Chico", 1, false, 8),
            new("Maive", "Rafseazz.RSVCP_Matcha_Latte", 1, false, 8),
            new("Louie", "Rafseazz.RSVCP_Hundred_Flavor_Doughnut", 1, false, 8),
            new("Irene", "Rafseazz.RSVCP_Strawberry_Lover_Pie", 1, false, 8),
            new("Sonny", "Rafseazz.RSVCP_Relic_Fox_Mask", 1, false, 10),
            new("Paula", "Rafseazz.RSVCP_Ridgeside_Shaketini", 1, false, 10),
            new("Carmen", "Rafseazz.RSVCP_Zesty_Tuna", 1, false, 10),
            new("Lorenzo", "Rafseazz.RSVCP_Arugula_Roll", 1, false, 10),
            new("Shanice", "Rafseazz.RSVCP_Ridgeside_Clementine", 1, false, 12),
            new("Anton", "Rafseazz.RSVCP_Ridge_Azorean_Flower", 1, false, 12),
            new("Faye", "Rafseazz.RSVCP_Forest_Amancay", 1, false, 12),
            new("Malaya", "Rafseazz.RSVCP_Shell_Bracelet", 1, false, 12),
            new("Kimpoi", "Rafseazz.RSVCP_Honey_Glazed_Salad", 1, false, 14),
            new("Naomi", "Rafseazz.RSVCP_Kek_s_Style_Shortcake", 1, false, 14),
            new("Jio", "Rafseazz.RSVCP_Kedi_Delight", 1, false, 14),
            new("Daia", "Rafseazz.RSVCP_Summit_Iced_Tea", 1, false, 14),
            new("Undreya", "Rafseazz.RSVCP_Elven_Comb", 1, false, 16),
            new("Zayne", "Rafseazz.RSVCP_Ginger_Rangpur_Meringue", 1, false, 16),
            new("Bryle", "Rafseazz.RSVCP_Summer_Mountain_Blessing", 1, false, 16),
        };

        // East Scarp (Lemurkat.EastScarpNPCs). Custom East Scarp items where loved; fruit/veg/flower loves are sent as seeds.
        private readonly WelcomeNpc[] _villagersEastScarp =
        {
            // Romanceable -> friendship gated
            new("Aideen", "429", 5, true, 0),                          // Jazz Seeds (loves Blue Jazz)
            new("ToriLK", "EastScarp_FrostBerry_Bliss_Balls", 1, true, 0),
            new("CorwinLK", "303", 1, true, 0),                        // Pale Ale
            new("DaleWaede", "614", 1, true, 0),                       // Ginger
            // Day gated (3-4 per day, offset from RSV days)
            new("Beatrice", "EastScarp_Biteback_Curry", 1, false, 3),
            new("Eyvinder", "EastScarp_Void_Goat_Cheese_Aged", 1, false, 3),
            new("EdithHart", "395", 1, false, 3),                      // Coffee
            new("EthanHart", "724", 1, false, 3),                      // Maple Syrup
            new("Jacob", "241", 1, false, 5),                          // Survival Burger
            new("Eloise", "EastScarp_Blue_Jay_Feather", 1, false, 5),
            new("Jasper", "72", 1, false, 5),                          // Diamond
            new("Jessie", "EastScarp_Cuccaroot_Seed", 5, false, 5),    // Cuccaroot Seeds (loves Cuccaroot)
            new("JosephineK", "348", 1, false, 7),                     // Wine
            new("Juliet", "453", 5, false, 7),                         // Poppy Seeds (loves Poppy)
            new("KeanuAvis", "242", 1, false, 7),                      // Dish O' The Sea
            new("MichaelHart", "EastScarp_SeaGlass", 1, false, 7),
            new("OliverK", "589", 1, false, 9),                        // Trilobite
            new("Rosa", "745", 5, false, 9),                           // Strawberry Seeds (loves Strawberry)
            new("VivienneLK", "EastScarp_Vintage_Ruby_Ring", 1, false, 9),
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

            foreach (var npc in _villagersExpanded)
            {
                var stringKey = GetMailStringKey(npc);
                var letterId = $"{Game1.uniqueIDForThisGame}.{ModManifest.UniqueID}.{stringKey}";
                MailRepository.SaveLetter(new Letter(
                    letterId,
                    GetMailContent(stringKey),
                    GetMailListOfItems(npc),
                    l=>!Game1.player.mailReceived.Contains(l.Id) && l.Id.Contains(Game1.uniqueIDForThisGame.ToString()) && new Switch<String, bool>(npc)
                        .Case("Olivia").Then(Game1.player.friendshipData.ContainsKey(npc) && Game1.player.friendshipData[npc]?.Points >= 150)
                        .Case("Sophia").Then(Game1.player.friendshipData.ContainsKey(npc) && Game1.player.friendshipData[npc]?.Points >= 150)
                        .Case("Victor").Then(Game1.player.friendshipData.ContainsKey(npc) && Game1.player.friendshipData[npc]?.Points >= 150)
                        .Case("Andy").Then(Game1.dayOfMonth > 5)
                        .Case("Gunther").Then(Game1.dayOfMonth > 11)
                        .Case("Morris").Then(Game1.dayOfMonth > 1)
                        .Default(false),
                    (l)=>Game1.player.mailReceived.Add(l.Id),
                    whichBG: npc == "Sandy" ? 1 : 0
                ));
            }

            if (Helper.ModRegistry.IsLoaded("Rafseazz.RidgesideVillage"))
                RegisterModVillagers(_villagersRidgeside);

            if (Helper.ModRegistry.IsLoaded("Lemurkat.EastScarpNPCs"))
                RegisterModVillagers(_villagersEastScarp);
        }

        private void RegisterModVillagers(IEnumerable<WelcomeNpc> villagers)
        {
            foreach (var villager in villagers)
            {
                var npc = villager;
                var stringKey = GetMailStringKey(npc.Name);
                var letterId = $"{Game1.uniqueIDForThisGame}.{ModManifest.UniqueID}.{stringKey}";
                MailRepository.SaveLetter(new Letter(
                    letterId,
                    GetMailContent(stringKey),
                    new List<Item> { ItemRegistry.Create($"(O){npc.ItemId}", npc.Quantity) },
                    l => !Game1.player.mailReceived.Contains(l.Id)
                         && l.Id.Contains(Game1.uniqueIDForThisGame.ToString())
                         && (npc.Romanceable
                             ? Game1.player.friendshipData.ContainsKey(npc.Name) && Game1.player.friendshipData[npc.Name]?.Points >= 150
                             : Game1.dayOfMonth > npc.Day),
                    l => Game1.player.mailReceived.Add(l.Id),
                    whichBG: 0
                ));
            }
        }

        private bool IsSdvExpandedLoaded => this.Helper.ModRegistry.IsLoaded("");

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
                // Expanded Villagers
                .Case("Andy").Then(new List<Item>() { new StardewValley.Object("22", 3) })
                .Case("Gunther").Then(new List<Item>() { new StardewValley.Object("535", 1) })
                .Case("Morris").Then(new List<Item>() { new StardewValley.Object("167", 3) })
                .Case("Olivia").Then(new List<Item>() { new StardewValley.Object("220", 1) })
                .Case("Sophia").Then(new List<Item>() { new StardewValley.Object("595", 1) })
                .Case("Victor").Then(new List<Item>() { new StardewValley.Object("709", 5) })
                .Default(new List<Item>());
        }
    }
}