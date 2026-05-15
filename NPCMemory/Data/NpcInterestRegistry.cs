using System.Collections.Generic;
using NPCMemory.Models;

namespace NPCMemory.Data
{
    public static class NpcInterestRegistry
    {
        private static readonly Dictionary<string, List<ActivityType>> _interests = new()
        {
            // Marriage candidates
            ["Alex"] = new() { ActivityType.Cooking },
            ["Elliott"] = new() { ActivityType.Fishing, ActivityType.Cooking },
            ["Harvey"] = new() { ActivityType.Cooking, ActivityType.ArtisanGoods, ActivityType.Farming, ActivityType.Beverages },
            ["Sam"] = new() { ActivityType.Mining, ActivityType.Cooking },
            ["Sebastian"] = new() { ActivityType.Mining, ActivityType.VoidItems, ActivityType.Combat },
            ["Shane"] = new() { ActivityType.Beverages, ActivityType.Cooking, ActivityType.AnimalCare },
            ["Abigail"] = new() { ActivityType.Mining, ActivityType.Combat, ActivityType.Sweets, ActivityType.Farming },
            ["Emily"] = new() { ActivityType.Mining, ActivityType.AnimalCare },
            ["Haley"] = new() { ActivityType.Flowers, ActivityType.Sweets, ActivityType.Cooking },
            ["Leah"] = new() { ActivityType.Foraging, ActivityType.Cooking, ActivityType.ArtisanGoods },
            ["Maru"] = new() { ActivityType.Technology, ActivityType.Mining, ActivityType.Farming, ActivityType.Cooking },
            ["Penny"] = new() { ActivityType.Cooking, ActivityType.Farming, ActivityType.Flowers },

            // Non-marriage NPCs
            ["Caroline"] = new() { ActivityType.Cooking, ActivityType.Beverages, ActivityType.Flowers },
            ["Clint"] = new() { ActivityType.Mining },
            ["Demetrius"] = new() { ActivityType.Technology, ActivityType.Farming },
            ["Dwarf"] = new() { ActivityType.Mining },
            ["Evelyn"] = new() { ActivityType.Flowers, ActivityType.Sweets, ActivityType.Farming },
            ["George"] = new() { ActivityType.Cooking, ActivityType.Farming },
            ["Gus"] = new() { ActivityType.Cooking, ActivityType.Fishing },
            ["Jas"] = new() { ActivityType.Sweets, ActivityType.Flowers },
            ["Jodi"] = new() { ActivityType.Cooking, ActivityType.Fishing, ActivityType.Farming },
            ["Kent"] = new() { ActivityType.Foraging, ActivityType.Cooking, ActivityType.Combat },
            ["Krobus"] = new() { ActivityType.VoidItems, ActivityType.Combat, ActivityType.Mining, ActivityType.Magic },
            ["Leo"] = new() { ActivityType.AnimalCare, ActivityType.Foraging },
            ["Lewis"] = new() { ActivityType.Cooking, ActivityType.Farming, ActivityType.Beverages },
            ["Linus"] = new() { ActivityType.Foraging, ActivityType.Fishing, ActivityType.Cooking },
            ["Marnie"] = new() { ActivityType.AnimalCare, ActivityType.Cooking, ActivityType.Farming },
            ["Pam"] = new() { ActivityType.Beverages, ActivityType.Cooking },
            ["Pierre"] = new() { ActivityType.Farming },
            ["Robin"] = new() { ActivityType.ArtisanGoods, ActivityType.Cooking, ActivityType.Farming },
            ["Sandy"] = new() { ActivityType.Flowers, ActivityType.Cooking },
            ["Vincent"] = new() { ActivityType.Sweets, ActivityType.AnimalCare },
            ["Willy"] = new() { ActivityType.Fishing, ActivityType.Mining },
            ["Wizard"] = new() { ActivityType.Magic, ActivityType.VoidItems }
        };

        public static List<ActivityType>? GetInterests(string npcName)
        {
            return _interests.TryGetValue(npcName, out var interests) ? interests : null;
        }

        public static IReadOnlyDictionary<string, List<ActivityType>> GetAll() => _interests;
    }
}
