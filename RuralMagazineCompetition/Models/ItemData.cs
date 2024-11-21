using RuralMagazineCompetition.Enums;
using RuralMagazineCompetition.Mapper;
using StardewValley;

namespace RuralMagazineCompetition.Models
{
    public static class ItemData
    {
        public static Dictionary<string, ItemInfo> Items { get; set; } = new ();

        static ItemData()
        {
            Create();
        }

        public static void Refresh()
        {
            Items.Clear();
            Create();
        } 

        private static void Create()
        {
            CreateCrops();
            CreateForages();
            CreateFishes();
            CreateAnimalProduces();
            CreateArtisanGoods();
            CreateMinerals();
        }

        private static void CreateCrops()
        {
            // Spring Crops
            Items.Add(ItemIdMapper.Crops.BlueJazz, new ItemInfo { BasePoints = 14, AvailableSeasons = new List<Season> { Season.Spring }, Category = CompetitionCategoryEnum.Crops});
            Items.Add(ItemIdMapper.Crops.Cauliflower, new ItemInfo { BasePoints = 30, AvailableSeasons = new List<Season> { Season.Spring }, Category = CompetitionCategoryEnum.Crops});
            Items.Add(ItemIdMapper.Crops.Garlic, new ItemInfo { BasePoints = 8, AvailableSeasons = new List<Season> { Season.Spring }, Category = CompetitionCategoryEnum.Crops});
            Items.Add(ItemIdMapper.Crops.Kale, new ItemInfo { BasePoints = 12, AvailableSeasons = new List<Season> { Season.Spring }, Category = CompetitionCategoryEnum.Crops});
            Items.Add(ItemIdMapper.Crops.Parsnip, new ItemInfo { BasePoints = 8, AvailableSeasons = new List<Season> { Season.Spring }, Category = CompetitionCategoryEnum.Crops});
            Items.Add(ItemIdMapper.Crops.Potato, new ItemInfo { BasePoints = 12, AvailableSeasons = new List<Season> { Season.Spring }, Category = CompetitionCategoryEnum.Crops});
            Items.Add(ItemIdMapper.Crops.Rhubarb, new ItemInfo { BasePoints = 33, AvailableSeasons = new List<Season> { Season.Spring }, Category = CompetitionCategoryEnum.Crops});
            Items.Add(ItemIdMapper.Crops.Tulip, new ItemInfo { BasePoints = 12, AvailableSeasons = new List<Season> { Season.Spring }, Category = CompetitionCategoryEnum.Crops});
            Items.Add(ItemIdMapper.Crops.Carrot, new ItemInfo { BasePoints = 5, AvailableSeasons = new List<Season> { Season.Spring }, Category = CompetitionCategoryEnum.Crops});
            Items.Add(ItemIdMapper.Crops.GreenBean, new ItemInfo { BasePoints = 21, AvailableSeasons = new List<Season> { Season.Spring }, Category = CompetitionCategoryEnum.Crops});
            Items.Add(ItemIdMapper.Crops.Strawberry, new ItemInfo { BasePoints = 21, AvailableSeasons = new List<Season> { Season.Spring }, Category = CompetitionCategoryEnum.Crops});
            
            // Summer Crops
            Items.Add(ItemIdMapper.Crops.Blueberry, new ItemInfo { BasePoints = 21, AvailableSeasons = new List<Season> { Season.Summer }, Category = CompetitionCategoryEnum.Crops});
            Items.Add(ItemIdMapper.Crops.Hops, new ItemInfo { BasePoints = 12, AvailableSeasons = new List<Season> { Season.Summer }, Category = CompetitionCategoryEnum.Crops});
            Items.Add(ItemIdMapper.Crops.Melon, new ItemInfo { BasePoints = 30, AvailableSeasons = new List<Season> { Season.Summer }, Category = CompetitionCategoryEnum.Crops});
            Items.Add(ItemIdMapper.Crops.Pepper, new ItemInfo { BasePoints = 10, AvailableSeasons = new List<Season> { Season.Summer }, Category = CompetitionCategoryEnum.Crops});
            Items.Add(ItemIdMapper.Crops.Radish, new ItemInfo { BasePoints = 8, AvailableSeasons = new List<Season> { Season.Summer }, Category = CompetitionCategoryEnum.Crops});
            Items.Add(ItemIdMapper.Crops.RedCabbage, new ItemInfo { BasePoints = 21, AvailableSeasons = new List<Season> { Season.Summer }, Category = CompetitionCategoryEnum.Crops});
            Items.Add(ItemIdMapper.Crops.Starfruit, new ItemInfo { BasePoints = 33, AvailableSeasons = new List<Season> { Season.Summer }, Category = CompetitionCategoryEnum.Crops});
            Items.Add(ItemIdMapper.Crops.Sunflower, new ItemInfo { BasePoints = 18, AvailableSeasons = new List<Season> { Season.Summer }, Category = CompetitionCategoryEnum.Crops});
            Items.Add(ItemIdMapper.Crops.SummerSpangle, new ItemInfo { BasePoints = 18, AvailableSeasons = new List<Season> { Season.Summer }, Category = CompetitionCategoryEnum.Crops});
            Items.Add(ItemIdMapper.Crops.Tomato, new ItemInfo { BasePoints = 24, AvailableSeasons = new List<Season> { Season.Summer }, Category = CompetitionCategoryEnum.Crops});
            Items.Add(ItemIdMapper.Crops.Wheat, new ItemInfo { BasePoints = 8, AvailableSeasons = new List<Season> { Season.Summer }, Category = CompetitionCategoryEnum.Crops});
            Items.Add(ItemIdMapper.Crops.SummerSquash, new ItemInfo { BasePoints = 8, AvailableSeasons = new List<Season> { Season.Summer }, Category = CompetitionCategoryEnum.Crops});
            
            // Fall Crops
            Items.Add(ItemIdMapper.Crops.Amaranth, new ItemInfo { BasePoints = 15, AvailableSeasons = new List<Season> { Season.Fall }, Category = CompetitionCategoryEnum.Crops});
            Items.Add(ItemIdMapper.Crops.Artichoke, new ItemInfo { BasePoints = 18, AvailableSeasons = new List<Season> { Season.Fall }, Category = CompetitionCategoryEnum.Crops});
            Items.Add(ItemIdMapper.Crops.Beet, new ItemInfo { BasePoints = 12, AvailableSeasons = new List<Season> { Season.Fall }, Category = CompetitionCategoryEnum.Crops});
            Items.Add(ItemIdMapper.Crops.Bokchoy, new ItemInfo { BasePoints = 8, AvailableSeasons = new List<Season> { Season.Fall }, Category = CompetitionCategoryEnum.Crops});
            Items.Add(ItemIdMapper.Crops.Corn, new ItemInfo { BasePoints = 24, AvailableSeasons = new List<Season> { Season.Fall }, Category = CompetitionCategoryEnum.Crops});
            Items.Add(ItemIdMapper.Crops.Cranberries, new ItemInfo { BasePoints = 8, AvailableSeasons = new List<Season> { Season.Fall }, Category = CompetitionCategoryEnum.Crops});
            Items.Add(ItemIdMapper.Crops.Eggplant, new ItemInfo { BasePoints = 5, AvailableSeasons = new List<Season> { Season.Fall }, Category = CompetitionCategoryEnum.Crops});
            Items.Add(ItemIdMapper.Crops.FairyRose, new ItemInfo { BasePoints = 30, AvailableSeasons = new List<Season> { Season.Fall }, Category = CompetitionCategoryEnum.Crops});
            Items.Add(ItemIdMapper.Crops.Pumpkin, new ItemInfo { BasePoints = 33, AvailableSeasons = new List<Season> { Season.Fall }, Category = CompetitionCategoryEnum.Crops});
            Items.Add(ItemIdMapper.Crops.Yam, new ItemInfo { BasePoints = 24, AvailableSeasons = new List<Season> { Season.Fall }, Category = CompetitionCategoryEnum.Crops});
            Items.Add(ItemIdMapper.Crops.Broccoli, new ItemInfo { BasePoints = 12, AvailableSeasons = new List<Season> { Season.Fall }, Category = CompetitionCategoryEnum.Crops});
            
            // Winter Crops
            Items.Add(ItemIdMapper.Crops.Powdermelon, new ItemInfo { BasePoints = 21, AvailableSeasons = new List<Season> { Season.Winter }, Category = CompetitionCategoryEnum.Crops});
        }
        // BasePoints -> 10 is default for 5 days for collecting
        // single harvest: 3 days -> 5 (points)
        // single harvest: 4 days -> 8 (points)
        // single harvest: 6 days -> 12 points
        // single harvest: 7 days -> 15 points
        // single harvest: 8 days -> 18 points
        // single harvest: 9 days -> 21 points
        // single harvest: 10 days -> 24 points
        // single harvest: 11 days -> 27 points
        // single harvest: 12 days -> 30 points
        // single harvest: 13 days -> 33 points

        private static void CreateForages()
        {
            // Spring Forages
            Items.Add(ItemIdMapper.Forages.Daffodil, new ItemInfo { BasePoints = 10, AvailableSeasons = new List<Season> { Season.Spring }, Category = CompetitionCategoryEnum.Forage});
            Items.Add(ItemIdMapper.Forages.Dandelion, new ItemInfo { BasePoints = 12, AvailableSeasons = new List<Season> { Season.Spring }, Category = CompetitionCategoryEnum.Forage});
            Items.Add(ItemIdMapper.Forages.Leek, new ItemInfo { BasePoints = 15, AvailableSeasons = new List<Season> { Season.Spring }, Category = CompetitionCategoryEnum.Forage});
            Items.Add(ItemIdMapper.Forages.SpringOnion, new ItemInfo { BasePoints = 5, AvailableSeasons = new List<Season> { Season.Spring }, Category = CompetitionCategoryEnum.Forage});
            Items.Add(ItemIdMapper.Forages.WildHorseradish, new ItemInfo { BasePoints = 8, AvailableSeasons = new List<Season> { Season.Spring }, Category = CompetitionCategoryEnum.Forage});
            
            // Summer Forages
            Items.Add(ItemIdMapper.Forages.SweetPea, new ItemInfo { BasePoints = 5, AvailableSeasons = new List<Season> { Season.Summer }, Category = CompetitionCategoryEnum.Forage});
            Items.Add(ItemIdMapper.Forages.FiddleheadFern, new ItemInfo { BasePoints = 12, AvailableSeasons = new List<Season> { Season.Summer }, Category = CompetitionCategoryEnum.Forage});
            Items.Add(ItemIdMapper.Forages.CactusFruit, new ItemInfo { BasePoints = 8, AvailableSeasons = new List<Season> { Season.Summer }, Category = CompetitionCategoryEnum.Forage});
            Items.Add(ItemIdMapper.Forages.Coconut, new ItemInfo { BasePoints = 15, AvailableSeasons = new List<Season> { Season.Summer }, Category = CompetitionCategoryEnum.Forage});
            Items.Add(ItemIdMapper.Forages.SpiceBerry, new ItemInfo { BasePoints = 10, AvailableSeasons = new List<Season> { Season.Summer }, Category = CompetitionCategoryEnum.Forage});
            
            // Fall Forages
            Items.Add(ItemIdMapper.Forages.Blackberry, new ItemInfo { BasePoints = 2, AvailableSeasons = new List<Season> { Season.Fall }, Category = CompetitionCategoryEnum.Forage});
            Items.Add(ItemIdMapper.Forages.CommonMushroom, new ItemInfo { BasePoints = 4, AvailableSeasons = new List<Season> { Season.Fall }, Category = CompetitionCategoryEnum.Forage});
            Items.Add(ItemIdMapper.Forages.Chanterelle, new ItemInfo { BasePoints = 16, AvailableSeasons = new List<Season> { Season.Fall }, Category = CompetitionCategoryEnum.Forage});
            Items.Add(ItemIdMapper.Forages.Hazelnut, new ItemInfo { BasePoints = 12, AvailableSeasons = new List<Season> { Season.Fall }, Category = CompetitionCategoryEnum.Forage});
            Items.Add(ItemIdMapper.Forages.WildPlum, new ItemInfo { BasePoints = 10, AvailableSeasons = new List<Season> { Season.Fall }, Category = CompetitionCategoryEnum.Forage});
            
            // Winter Forages
            Items.Add(ItemIdMapper.Forages.Crocus, new ItemInfo { BasePoints = 7, AvailableSeasons = new List<Season> { Season.Winter }, Category = CompetitionCategoryEnum.Forage});
            Items.Add(ItemIdMapper.Forages.CrystalFruit, new ItemInfo { BasePoints = 14, AvailableSeasons = new List<Season> { Season.Winter }, Category = CompetitionCategoryEnum.Forage});
            Items.Add(ItemIdMapper.Forages.Holly, new ItemInfo { BasePoints = 10, AvailableSeasons = new List<Season> { Season.Winter }, Category = CompetitionCategoryEnum.Forage});
            Items.Add(ItemIdMapper.Forages.SnowYam, new ItemInfo { BasePoints = 12, AvailableSeasons = new List<Season> { Season.Winter }, Category = CompetitionCategoryEnum.Forage});
            Items.Add(ItemIdMapper.Forages.WinterRoot, new ItemInfo { BasePoints = 9, AvailableSeasons = new List<Season> { Season.Winter }, Category = CompetitionCategoryEnum.Forage});
        }

        private static void CreateFishes()
        {
            // Spring (ONLY) Fishes
            
            // Summer (ONLY) Fishes
            Items.Add(ItemIdMapper.Fishes.Dorado, new ItemInfo { BasePoints = 29, AvailableSeasons = new List<Season> { Season.Summer }, Category = CompetitionCategoryEnum.Fish});
            Items.Add(ItemIdMapper.Fishes.Octopus, new ItemInfo { BasePoints = 34, AvailableSeasons = new List<Season> { Season.Summer }, Category = CompetitionCategoryEnum.Fish});
            Items.Add(ItemIdMapper.Fishes.Pufferfish, new ItemInfo { BasePoints = 29, AvailableSeasons = new List<Season> { Season.Summer }, Category = CompetitionCategoryEnum.Fish});
            Items.Add(ItemIdMapper.Fishes.RainbowTrout, new ItemInfo { BasePoints = 18, AvailableSeasons = new List<Season> { Season.Summer }, Category = CompetitionCategoryEnum.Fish});
            
            // Fall (ONLY) Fishes
            Items.Add(ItemIdMapper.Fishes.Salmon, new ItemInfo { BasePoints = 19, AvailableSeasons = new List<Season> { Season.Fall }, Category = CompetitionCategoryEnum.Fish});
            
            // Winter (ONLY) Fishes
            Items.Add(ItemIdMapper.Fishes.Blobfish, new ItemInfo { BasePoints = 28, AvailableSeasons = new List<Season> { Season.Winter }, Category = CompetitionCategoryEnum.Fish});
            Items.Add(ItemIdMapper.Fishes.Lingcod, new ItemInfo { BasePoints = 31, AvailableSeasons = new List<Season> { Season.Winter }, Category = CompetitionCategoryEnum.Fish});
            Items.Add(ItemIdMapper.Fishes.MidnightSquid, new ItemInfo { BasePoints = 21, AvailableSeasons = new List<Season> { Season.Winter }, Category = CompetitionCategoryEnum.Fish});
            Items.Add(ItemIdMapper.Fishes.Perch, new ItemInfo { BasePoints = 14, AvailableSeasons = new List<Season> { Season.Winter }, Category = CompetitionCategoryEnum.Fish});
            Items.Add(ItemIdMapper.Fishes.SpookyFish, new ItemInfo { BasePoints = 23, AvailableSeasons = new List<Season> { Season.Winter }, Category = CompetitionCategoryEnum.Fish});
            Items.Add(ItemIdMapper.Fishes.Squid, new ItemInfo { BasePoints = 28, AvailableSeasons = new List<Season> { Season.Winter }, Category = CompetitionCategoryEnum.Fish});
            
            // Spring and Summer Fishes
            Items.Add(ItemIdMapper.Fishes.Flounder, new ItemInfo { BasePoints = 19, AvailableSeasons = new List<Season> { Season.Spring, Season.Summer }, Category = CompetitionCategoryEnum.Fish});
            Items.Add(ItemIdMapper.Fishes.Sunfish, new ItemInfo { BasePoints = 13, AvailableSeasons = new List<Season> { Season.Spring, Season.Summer }, Category = CompetitionCategoryEnum.Fish});
            
            // Spring and Fall Fishes
            Items.Add(ItemIdMapper.Fishes.Anchovy, new ItemInfo { BasePoints = 13, AvailableSeasons = new List<Season> { Season.Spring, Season.Fall }, Category = CompetitionCategoryEnum.Fish});
            Items.Add(ItemIdMapper.Fishes.Eel, new ItemInfo { BasePoints = 26, AvailableSeasons = new List<Season> { Season.Spring, Season.Fall }, Category = CompetitionCategoryEnum.Fish});
            Items.Add(ItemIdMapper.Fishes.SmallmouthBass, new ItemInfo { BasePoints = 12, AvailableSeasons = new List<Season> { Season.Spring, Season.Fall }, Category = CompetitionCategoryEnum.Fish});
            
            // Spring and Winter Fishes
            Items.Add(ItemIdMapper.Fishes.Herring, new ItemInfo { BasePoints = 11, AvailableSeasons = new List<Season> { Season.Spring, Season.Winter }, Category = CompetitionCategoryEnum.Fish});
            
            // Summer and Fall Fishes
            Items.Add(ItemIdMapper.Fishes.SuperCucumber, new ItemInfo { BasePoints = 29, AvailableSeasons = new List<Season> { Season.Summer, Season.Fall }, Category = CompetitionCategoryEnum.Fish});
            Items.Add(ItemIdMapper.Fishes.Tilapia, new ItemInfo { BasePoints = 19, AvailableSeasons = new List<Season> { Season.Summer, Season.Fall }, Category = CompetitionCategoryEnum.Fish});
            
            // Summer and Winter Fishes
            Items.Add(ItemIdMapper.Fishes.Pike, new ItemInfo { BasePoints = 23, AvailableSeasons = new List<Season> { Season.Summer, Season.Winter }, Category = CompetitionCategoryEnum.Fish});
            Items.Add(ItemIdMapper.Fishes.RedMullet, new ItemInfo { BasePoints = 21, AvailableSeasons = new List<Season> { Season.Summer, Season.Winter }, Category = CompetitionCategoryEnum.Fish});
            Items.Add(ItemIdMapper.Fishes.Sturgeon, new ItemInfo { BasePoints = 29, AvailableSeasons = new List<Season> { Season.Summer, Season.Winter }, Category = CompetitionCategoryEnum.Fish});
            Items.Add(ItemIdMapper.Fishes.Tuna, new ItemInfo { BasePoints = 26, AvailableSeasons = new List<Season> { Season.Summer, Season.Winter }, Category = CompetitionCategoryEnum.Fish});
            
            // Fall and Winter Fishes
            Items.Add(ItemIdMapper.Fishes.Albacore, new ItemInfo { BasePoints = 23, AvailableSeasons = new List<Season> { Season.Fall, Season.Winter }, Category = CompetitionCategoryEnum.Fish});
            Items.Add(ItemIdMapper.Fishes.MidnightCarp, new ItemInfo { BasePoints = 21, AvailableSeasons = new List<Season> { Season.Fall, Season.Winter }, Category = CompetitionCategoryEnum.Fish});
            Items.Add(ItemIdMapper.Fishes.SeaCucumber, new ItemInfo { BasePoints = 16, AvailableSeasons = new List<Season> { Season.Fall, Season.Winter }, Category = CompetitionCategoryEnum.Fish});
            Items.Add(ItemIdMapper.Fishes.TigerTrout, new ItemInfo { BasePoints = 23, AvailableSeasons = new List<Season> { Season.Fall, Season.Winter }, Category = CompetitionCategoryEnum.Fish});
            Items.Add(ItemIdMapper.Fishes.Walleye, new ItemInfo { BasePoints = 18, AvailableSeasons = new List<Season> { Season.Fall, Season.Winter }, Category = CompetitionCategoryEnum.Fish});
            
            // Spring, Summer, and Fall
            Items.Add(ItemIdMapper.Fishes.Catfish, new ItemInfo { BasePoints = 28, AvailableSeasons = new List<Season> { Season.Spring, Season.Summer, Season.Fall }, Category = CompetitionCategoryEnum.Fish});
            Items.Add(ItemIdMapper.Fishes.Shad, new ItemInfo { BasePoints = 18, AvailableSeasons = new List<Season> { Season.Spring, Season.Summer, Season.Fall }, Category = CompetitionCategoryEnum.Fish});
            
            // Spring, Summer, and Winter
            Items.Add(ItemIdMapper.Fishes.Halibut, new ItemInfo { BasePoints = 19, AvailableSeasons = new List<Season> { Season.Spring, Season.Summer, Season.Winter }, Category = CompetitionCategoryEnum.Fish});
            
            // Spring, Fall, and Winter
            Items.Add(ItemIdMapper.Fishes.Sardine, new ItemInfo { BasePoints = 13, AvailableSeasons = new List<Season> { Season.Spring, Season.Fall, Season.Winter }, Category = CompetitionCategoryEnum.Fish});
            
            // Summer, Fall, and Winter
            Items.Add(ItemIdMapper.Fishes.RedSnapper, new ItemInfo { BasePoints = 16, AvailableSeasons = new List<Season> { Season.Spring, Season.Summer }, Category = CompetitionCategoryEnum.Fish});
            
            // All Seasons
            Items.Add(ItemIdMapper.Fishes.Bream, new ItemInfo { BasePoints = 14, AvailableSeasons = new List<Season> { Season.Spring, Season.Summer, Season.Fall, Season.Winter }, Category = CompetitionCategoryEnum.Fish});
            Items.Add(ItemIdMapper.Fishes.Bullhead, new ItemInfo { BasePoints = 18, AvailableSeasons = new List<Season> { Season.Spring, Season.Summer, Season.Fall, Season.Winter }, Category = CompetitionCategoryEnum.Fish});
            Items.Add(ItemIdMapper.Fishes.Chub, new ItemInfo { BasePoints = 14, AvailableSeasons = new List<Season> { Season.Spring, Season.Summer, Season.Fall, Season.Winter }, Category = CompetitionCategoryEnum.Fish});
            Items.Add(ItemIdMapper.Fishes.IcePip, new ItemInfo { BasePoints = 31, AvailableSeasons = new List<Season> { Season.Spring, Season.Summer, Season.Fall, Season.Winter }, Category = CompetitionCategoryEnum.Fish});
            Items.Add(ItemIdMapper.Fishes.Ghostfish, new ItemInfo { BasePoints = 19, AvailableSeasons = new List<Season> { Season.Spring, Season.Summer, Season.Fall, Season.Winter }, Category = CompetitionCategoryEnum.Fish});
            Items.Add(ItemIdMapper.Fishes.Goby, new ItemInfo { BasePoints = 21, AvailableSeasons = new List<Season> { Season.Spring, Season.Summer, Season.Fall, Season.Winter }, Category = CompetitionCategoryEnum.Fish});
            Items.Add(ItemIdMapper.Fishes.LargemouthBass, new ItemInfo { BasePoints = 19, AvailableSeasons = new List<Season> { Season.Spring, Season.Summer, Season.Fall, Season.Winter }, Category = CompetitionCategoryEnum.Fish});
            Items.Add(ItemIdMapper.Fishes.LavaEel, new ItemInfo { BasePoints = 33, AvailableSeasons = new List<Season> { Season.Spring, Season.Summer, Season.Fall, Season.Winter }, Category = CompetitionCategoryEnum.Fish});
            Items.Add(ItemIdMapper.Fishes.SandFish, new ItemInfo { BasePoints = 24, AvailableSeasons = new List<Season> { Season.Spring, Season.Summer, Season.Fall, Season.Winter }, Category = CompetitionCategoryEnum.Fish});
            Items.Add(ItemIdMapper.Fishes.ScorpionCarp, new ItemInfo { BasePoints = 33, AvailableSeasons = new List<Season> { Season.Spring, Season.Summer, Season.Fall, Season.Winter }, Category = CompetitionCategoryEnum.Fish});
            Items.Add(ItemIdMapper.Fishes.Stonefish, new ItemInfo { BasePoints = 24, AvailableSeasons = new List<Season> { Season.Spring, Season.Summer, Season.Fall, Season.Winter }, Category = CompetitionCategoryEnum.Fish});
            Items.Add(ItemIdMapper.Fishes.Woodskip, new ItemInfo { BasePoints = 19, AvailableSeasons = new List<Season> { Season.Spring, Season.Summer, Season.Fall, Season.Winter }, Category = CompetitionCategoryEnum.Fish});
            
            // Legend Fishes
            Items.Add(ItemIdMapper.Fishes.Angler, new ItemInfo { BasePoints = 155, AvailableSeasons = new List<Season> { Season.Fall }, Category = CompetitionCategoryEnum.Fish});
            Items.Add(ItemIdMapper.Fishes.CrimsonFish, new ItemInfo { BasePoints = 170, AvailableSeasons = new List<Season> { Season.Summer }, Category = CompetitionCategoryEnum.Fish});
            Items.Add(ItemIdMapper.Fishes.Glacierfish, new ItemInfo { BasePoints = 180, AvailableSeasons = new List<Season> { Season.Winter }, Category = CompetitionCategoryEnum.Fish});
            Items.Add(ItemIdMapper.Fishes.Legend, new ItemInfo { BasePoints = 195, AvailableSeasons = new List<Season> { Season.Spring }, Category = CompetitionCategoryEnum.Fish});
            Items.Add(ItemIdMapper.Fishes.MutantCarp, new ItemInfo { BasePoints = 145, AvailableSeasons = new List<Season> { Season.Spring, Season.Summer, Season.Fall, Season.Winter }, Category = CompetitionCategoryEnum.Fish});
        }

        private static void CreateArtisanGoods()
        {
            var allArtisans = ItemIdMapper.ArtisanGoods.GetAll();

            foreach (var season in Enum.GetValues(typeof(Season)).Cast<Season>())
            {
                for (var i = 0; i < 5; i++)
                {
                    var random = Random.Shared.Next(allArtisans.Count);
                    var itemId = allArtisans[random];
                    Items.Add(itemId, new ItemInfo { BasePoints = ItemIdMapper.ArtisanGoods.GetBasePoints(itemId), AvailableSeasons = new List<Season> { season }, Category = CompetitionCategoryEnum.ArtisanGoods});
                    allArtisans.Remove(itemId);
                }
            }
        }

        private static void CreateAnimalProduces()
        {
            var allAnimalProduces = ItemIdMapper.AnimalProduces.GetAll();
            foreach (var season in Enum.GetValues(typeof(Season)).Cast<Season>())
            {
                for (var i = 0; i < 4; i++) 
                {
                    var random = Random.Shared.Next(allAnimalProduces.Count);
                    var itemId = allAnimalProduces[random];
                    Items.Add(itemId, new ItemInfo { BasePoints = ItemIdMapper.AnimalProduces.GetBasePoints(itemId), AvailableSeasons = new List<Season> { season }, Category = CompetitionCategoryEnum.AnimalProduce});
                    allAnimalProduces.Remove(itemId);
                }
            }
        }

        private static void CreateMinerals()
        {
            var allMinerals = ItemIdMapper.Minerals.GetAll();
            foreach (var season in Enum.GetValues(typeof(Season)).Cast<Season>())
            {
                for (var i = 0; i < 6; i++)
                {
                    var random = Random.Shared.Next(allMinerals.Count);
                    var itemId = allMinerals[random];
                    Items.Add(itemId, new ItemInfo { BasePoints = ItemIdMapper.Minerals.GetBasePoints(itemId), AvailableSeasons = new List<Season> { season }, Category = CompetitionCategoryEnum.Minerals});
                    allMinerals.Remove(itemId);
                }
            }
        }
    }
}