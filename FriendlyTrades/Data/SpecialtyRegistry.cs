using System.Collections.Generic;
using FriendlyTrades.Models;

namespace FriendlyTrades.Data
{
    public class SpecialtyRegistry
    {
        private readonly Dictionary<string, NpcSpecialty> _specialties = new();
        private readonly Dictionary<string, string> _shopIdToNpc = new();

        public SpecialtyRegistry()
        {
            RegisterAll();
        }

        public bool HasSpecialty(string npcName) => _specialties.ContainsKey(npcName);

        public NpcSpecialty? GetSpecialty(string npcName) =>
            _specialties.TryGetValue(npcName, out var specialty) ? specialty : null;

        public string? GetNpcNameByShopId(string shopId) =>
            _shopIdToNpc.TryGetValue(shopId, out var npcName) ? npcName : null;

        private void RegisterAll()
        {
            // Clint — Blacksmith: Ores, Bars, Gems, Minerals, Geodes
            Register(new NpcSpecialty
            {
                NpcName = "Clint",
                SpecialtyKey = "clint",
                ShopId = "Blacksmith",
                ShopLocationName = "Blacksmith",
                AcceptedCategories = new HashSet<int> { -2, -12, -15 }, // gems, minerals, metal resources
                AcceptedItemIds = new HashSet<string>
                {
                    "535", "536", "537", "749", // Geode, Frozen Geode, Magma Geode, Omni Geode
                    "382" // Coal
                }
            });

            // Marnie — Rancher: Animal Products (raw + processed)
            Register(new NpcSpecialty
            {
                NpcName = "Marnie",
                SpecialtyKey = "marnie",
                ShopId = "AnimalShop",
                ShopLocationName = "AnimalShop",
                AcceptedCategories = new HashSet<int> { -5, -6 }, // eggs, milk
                AcceptedItemIds = new HashSet<string>
                {
                    "440", "444", "446", "430",       // Wool, Duck Feather, Rabbit's Foot, Truffle
                    "424", "426", "428",               // Cheese, Goat Cheese, Cloth
                    "306", "307", "308", "807",        // Mayonnaise, Duck Mayo, Void Mayo, Dino Mayo
                    "432",                             // Truffle Oil
                    "Butter"                           // Butter (SDV 1.6)
                }
            });

            // Gus — Chef: Cooked Food, Beverages & Gold+ Quality Crops
            Register(new NpcSpecialty
            {
                NpcName = "Gus",
                SpecialtyKey = "gus",
                ShopId = "Saloon",
                ShopLocationName = "Saloon",
                AcceptedCategories = new HashSet<int> { -7 }, // cooking
                AcceptedItemIds = new HashSet<string>
                {
                    "395", "253", "614" // Coffee, Triple Shot Espresso, Green Tea
                },
                QualityMinCategories = new Dictionary<int, int>
                {
                    { -75, 2 }, // Vegetables — gold quality or better
                    { -79, 2 }  // Fruits — gold quality or better
                }
            });

            // Willy — Fisherman: Fish, Crab Pot, Roe
            Register(new NpcSpecialty
            {
                NpcName = "Willy",
                SpecialtyKey = "willy",
                ShopId = "FishShop",
                ShopLocationName = "FishShop",
                AcceptedCategories = new HashSet<int> { -4 }, // fish
                AcceptedItemIds = new HashSet<string>
                {
                    "812", "447", "445" // Roe, Aged Roe, Caviar
                }
            });

            // Robin — Carpenter: Building Materials, Fences & Crafted Structures
            Register(new NpcSpecialty
            {
                NpcName = "Robin",
                SpecialtyKey = "robin",
                ShopId = "Carpenter",
                ShopLocationName = "ScienceHouse",
                AcceptedCategories = new HashSet<int> { -16 }, // building resources
                AcceptedItemIds = new HashSet<string>
                {
                    "388", "709", "390", "330", "771", "92", // Wood, Hardwood, Stone, Clay, Fiber, Sap
                    "Moss",                                    // Moss (SDV 1.6)
                    "322", "323", "324", "298", "325",         // Wood/Stone/Iron/Hardwood Fence, Gate
                    "93",                                      // Torch
                    "328", "329",                              // Wood Fence (alt), Stone Fence (alt)
                    "401"                                      // Straw Floor
                }
            });

            // Pierre — Farmer: Crops, Fruits, Flowers
            Register(new NpcSpecialty
            {
                NpcName = "Pierre",
                SpecialtyKey = "pierre",
                ShopId = "SeedShop",
                ShopLocationName = "SeedShop",
                AcceptedCategories = new HashSet<int> { -75, -79, -80 }, // vegetables, fruits, flowers
                AcceptedItemIds = new HashSet<string>()
            });

            // Krobus — Shadow Trader: Void & Monster Items
            Register(new NpcSpecialty
            {
                NpcName = "Krobus",
                SpecialtyKey = "krobus",
                ShopId = "ShadowShop",
                ShopLocationName = "Sewer",
                AcceptedCategories = new HashSet<int> { -28 }, // monster loot
                AcceptedItemIds = new HashSet<string>
                {
                    "769", "768",       // Void Essence, Solar Essence
                    "305", "308", "795" // Void Egg, Void Mayonnaise, Void Salmon
                }
            });
        }

        private void Register(NpcSpecialty specialty)
        {
            _specialties[specialty.NpcName] = specialty;
            if (!string.IsNullOrEmpty(specialty.ShopId))
                _shopIdToNpc[specialty.ShopId] = specialty.NpcName;
        }
    }
}
