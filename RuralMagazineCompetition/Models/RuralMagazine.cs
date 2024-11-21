using RuralMagazineCompetition.Enums;
using RuralMagazineCompetition.Models.Reward;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Inventories;

namespace RuralMagazineCompetition.Models
{
    public class RuralMagazine
    {
        private List<PrizeCategoryData> _competitionCategories = new ();
        private Dictionary<CompetitionCategoryEnum, int> _monthlyPoints = new ();
        private Dictionary<string, ItemInfo> _monthData = new();
            
        public RuralMagazine()
        {
            Initialize();
        }
        
        public List<PrizeCategoryData> GetCompetitionCategories()
        {
            return _competitionCategories;
        }
        
        public Dictionary<CompetitionCategoryEnum, int> GetMonthlyPoints()
        {
            return _monthlyPoints;
        }

        private void Initialize()
        {
            _monthData = ItemData.Items;
            // Todo: include prizes
            foreach (var key in Enum.GetValues(typeof(CompetitionCategoryEnum)).Cast<CompetitionCategoryEnum>())
            {
                _monthlyPoints.Add(key, 0);
            }
            // Initialize the list with competition category data
            _competitionCategories.Add(new PrizeCategoryData
            {
                Category = CompetitionCategoryEnum.Crops,
                PrizeTiers = new List<PrizeTier>
                {
                    // new PrizeTier { MinPoints = 0, MaxPoints = 99, Prize = new List<IReward> { /* Prizes for this tier */ } },
                    // new PrizeTier { MinPoints = 100, MaxPoints = 199, Prize = new List<IReward> { /* Prizes for this tier */ } },
                    // ... other tiers
                }
            });

            _competitionCategories.Add(new PrizeCategoryData
            {
                Category = CompetitionCategoryEnum.ArtisanGoods,
                PrizeTiers = new List<PrizeTier>
                {
                    // ... prize tiers for ArtisanGoods
                }
            });

            // ... add data for other categories
        }

        public void Refresh()
        {
            ItemData.Refresh();
            _monthData = ItemData.Items;
            foreach (var key in Enum.GetValues(typeof(CompetitionCategoryEnum)).Cast<CompetitionCategoryEnum>())
            {
                _monthlyPoints[key] = 0;
            }
        }
        
        public IReward SelectPrize(CompetitionCategoryEnum category, int points)
        {
            PrizeCategoryData categoryData = _competitionCategories.FirstOrDefault(c => c.Category == category);

            if (categoryData != null)
            {
                foreach (PrizeTier tier in categoryData.PrizeTiers)
                {
                    if (points >= tier.MinPoints && points < tier.MaxPoints)
                    {
                        return tier.Prize;
                    }
                }
            }

            return null; // Or a default prize if no tier is matched
        }
        
        public void UpdateShippedItems(IInventory inventory, Season season)
        {
            /*
            foreach (var itemSold in inventory)
            {
                if (itemSold != null)
                {
                    string id = itemSold.ItemId;
                    int quantitySold = itemSold.Stack;
                    int quality = itemSold.Quality; // 0 (lowQuality) | 1 (medQuality) | 2 (highQuality) | 4 (bestQuality)

                    ItemData.Items.TryGetValue(id, out ItemInfo itemInfo);
                    if (itemInfo != null && itemInfo.AvailableSeasons.Contains(season))
                    {
                        points[itemInfo.Category] += (quality + 1) * quantitySold;
                    }
                }
            }
            */
            // Get the relevant items for the current season
            var relevantItems = _monthData.Where(item => item.Value.AvailableSeasons.Contains(season));

            // Iterate through the items in the inventory
            for (var i = 0; i < inventory.Count; i++)
            {
                var item = inventory[i];

                if (item != null)
                {
                    var id = item.ItemId;
                    var quantitySold = item.Stack;
                    var quality = item.Quality;
                    ModEntry.Instance.Monitor.Log($"Item {id} has sold x{quantitySold} with a quality of {quality}", LogLevel.Info);

                    // Check if the item is relevant for the current season
                    if (relevantItems.Any(itemData => itemData.Key == id))
                    {
                        // Get the item info
                        _monthData.TryGetValue(id, out ItemInfo itemInfo);

                        // Update points
                        if (itemInfo != null)
                        {
                            _monthlyPoints[itemInfo.Category] += (quality + 1) * quantitySold * itemInfo.BasePoints;
                            ModEntry.Instance.Monitor.Log($"Monthly Points[{itemInfo.Category.ToString()}] = {_monthlyPoints[itemInfo.Category]}.", LogLevel.Info);
                        }
                    }
                }
            }
        }
    }
}