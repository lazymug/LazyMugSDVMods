using System.Collections.Generic;
using System.Linq;
using NPCMemory.Models;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace NPCMemory.Data
{
    public class ActivityTracker
    {
        private readonly IMonitor _monitor;
        private readonly ModConfig _config;

        private DailyActivity _today = new();

        // SDV item categories
        private const int FishCategory = -4;
        private const int EggCategory = -5;
        private const int MilkCategory = -6;
        private const int CookingCategory = -7;
        private const int MineralCategory = -12;
        private const int MetalResourceCategory = -15;
        private const int BuildingResourceCategory = -16;
        private const int MonsterLootCategory = -28;
        private const int FlowerCategory = -80;
        private const int FruitCategory = -79;
        private const int VegetableCategory = -75;
        private const int GemCategory = -2;
        private const int GreensCategory = -81;
        private const int ArtisanCategory = -26;

        // Specific item IDs for special tracking
        private static readonly HashSet<string> BeverageIds = new()
        {
            "346", // Beer
            "303", // Pale Ale
            "459", // Mead
            "348", // Wine
            "395", // Coffee
            "614", // Green Tea
            "253"  // Triple Shot Espresso
        };

        private static readonly HashSet<string> SweetIds = new()
        {
            "220", // Chocolate Cake
            "221", // Pink Cake
            "223", // Cookie
            "234", // Blueberry Tart
            "611", // Blackberry Cobbler
            "243"  // Miner's Treat
        };

        private static readonly HashSet<string> VoidItemIds = new()
        {
            "305", // Void Egg
            "308", // Void Mayonnaise
            "769", // Void Essence
            "795"  // Void Salmon
        };

        private static readonly HashSet<string> MagicItemIds = new()
        {
            "768", // Solar Essence
            "769", // Void Essence
            "886", // Bone Fragment (mystic)
        };

        private static readonly HashSet<string> TechnologyIds = new()
        {
            "787", // Battery Pack
            "337", // Iridium Bar
            "336", // Gold Bar
            "335", // Iron Bar
            "334"  // Copper Bar
        };

        private static readonly HashSet<string> ForageIds = new()
        {
            "16",  // Wild Horseradish
            "18",  // Daffodil
            "20",  // Leek
            "22",  // Dandelion
            "78",  // Cave Carrot
            "88",  // Coconut
            "90",  // Cactus Fruit
            "257", // Morel
            "259", // Fiddlehead Fern
            "281", // Chanterelle
            "283", // Holly
            "296", // Salmonberry
            "396", // Spice Berry
            "398", // Grape (wild)
            "402", // Sweet Pea
            "404", // Common Mushroom
            "406", // Wild Plum
            "408", // Hazelnut
            "410", // Blackberry
            "412", // Winter Root
            "414", // Crystal Fruit
            "416", // Snow Yam
            "418", // Crocus
            "420", // Red Mushroom
            "422"  // Purple Mushroom
        };

        private static readonly HashSet<string> MineLocations = new()
        {
            "UndergroundMine", "SkullCave", "VolcanoDungeon"
        };

        public ActivityTracker(IMonitor monitor, ModConfig config)
        {
            _monitor = monitor;
            _config = config;
        }

        public void StartNewDay()
        {
            _today = new DailyActivity
            {
                Day = Game1.dayOfMonth,
                Season = Game1.currentSeason,
                Year = Game1.year
            };
        }

        public DailyActivity GetTodayActivity() => _today;

        public void OnInventoryChanged(InventoryChangedEventArgs e)
        {
            foreach (var added in e.Added)
            {
                if (added is not StardewValley.Object obj)
                    continue;

                string itemId = obj.ItemId;
                int category = obj.Category;
                int stack = obj.Stack;

                // Fish
                if (category == FishCategory)
                    Track(ActivityType.Fishing, stack, obj.DisplayName);

                // Crops (vegetables, fruits)
                else if (category == VegetableCategory || category == FruitCategory)
                    Track(ActivityType.Farming, stack, obj.DisplayName);

                // Flowers
                else if (category == FlowerCategory)
                    Track(ActivityType.Flowers, stack, obj.DisplayName);

                // Cooking
                else if (category == CookingCategory)
                    Track(ActivityType.Cooking, stack, obj.DisplayName);

                // Gems and minerals
                else if (category == GemCategory || category == MineralCategory || category == MetalResourceCategory)
                    Track(ActivityType.Mining, stack, obj.DisplayName);

                // Monster loot
                else if (category == MonsterLootCategory)
                    Track(ActivityType.Combat, stack, obj.DisplayName);

                // Eggs, milk = animal care
                else if (category == EggCategory || category == MilkCategory)
                    Track(ActivityType.AnimalCare, stack, obj.DisplayName);

                // Artisan goods
                else if (category == ArtisanCategory)
                    Track(ActivityType.ArtisanGoods, stack, obj.DisplayName);

                // Forage
                else if (category == GreensCategory || ForageIds.Contains(itemId))
                    Track(ActivityType.Foraging, stack, obj.DisplayName);

                // Specific item checks (an item can trigger multiple activities)
                if (BeverageIds.Contains(itemId))
                    Track(ActivityType.Beverages, stack, obj.DisplayName);

                if (SweetIds.Contains(itemId))
                    Track(ActivityType.Sweets, stack, obj.DisplayName);

                if (VoidItemIds.Contains(itemId))
                    Track(ActivityType.VoidItems, stack, obj.DisplayName);

                if (MagicItemIds.Contains(itemId))
                    Track(ActivityType.Magic, stack, obj.DisplayName);

                if (TechnologyIds.Contains(itemId))
                    Track(ActivityType.Technology, stack, obj.DisplayName);
            }
        }

        public void OnWarped(WarpedEventArgs e)
        {
            string locationName = e.NewLocation.Name;

            // Mine locations — track mining activity
            if (MineLocations.Any(m => locationName.StartsWith(m)))
                Track(ActivityType.Mining, 1);
        }

        public void SimulateActivity(ActivityType type, int count, string? itemName = null)
        {
            Track(type, count, itemName);
        }

        public void SimulateAll()
        {
            Track(ActivityType.Fishing, 8, "Catfish");
            Track(ActivityType.Fishing, 0, "Salmon");
            Track(ActivityType.Mining, 12, "Amethyst");
            Track(ActivityType.Mining, 0, "Gold Ore");
            Track(ActivityType.Farming, 15, "Pumpkin");
            Track(ActivityType.Farming, 0, "Cauliflower");
            Track(ActivityType.Cooking, 3, "Salmon Dinner");
            Track(ActivityType.Combat, 10, "Bug Meat");
            Track(ActivityType.Foraging, 6, "Wild Horseradish");
            Track(ActivityType.AnimalCare, 4, "Large Egg");
            Track(ActivityType.ArtisanGoods, 5, "Wine");
            Track(ActivityType.Flowers, 3, "Sunflower");
            Track(ActivityType.Beverages, 2, "Beer");
            Track(ActivityType.Sweets, 2, "Chocolate Cake");
            Track(ActivityType.Technology, 3, "Battery Pack");
            Track(ActivityType.VoidItems, 1, "Void Egg");
            Track(ActivityType.Magic, 2, "Solar Essence");
        }

        private void Track(ActivityType type, int count, string? itemName = null)
        {
            if (!_today.Activities.TryGetValue(type, out var data))
            {
                data = new ActivityData();
                _today.Activities[type] = data;
            }

            data.Count += count;

            if (itemName != null && !data.NotableItems.Contains(itemName) && data.NotableItems.Count < 5)
                data.NotableItems.Add(itemName);

            if (_config.ShowDebugMessages)
                _monitor.Log($"[ActivityTracker] {type}: +{count} ({itemName ?? "no item"}), total: {data.Count}", LogLevel.Debug);
        }
    }
}
