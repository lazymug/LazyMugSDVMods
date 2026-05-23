using System.Collections.Generic;
using WeatheredLetters.Models;

namespace WeatheredLetters.Data
{
    public static class LetterRegistry
    {
        public static readonly List<LetterData> AllLetters = new()
        {
            // Spring — Grandpa's beginnings
            new LetterData
            {
                Id = "grandpa_01", Author = "Grandpa", Order = 1,
                TranslationKey = "letter.grandpa_01",
                LocationType = SpawnLocationType.Farm, SeasonHint = "spring"
            },
            new LetterData
            {
                Id = "grandpa_02", Author = "Grandpa", Order = 2,
                TranslationKey = "letter.grandpa_02",
                LocationType = SpawnLocationType.Farm, SeasonHint = "spring"
            },
            new LetterData
            {
                Id = "lewis_01", Author = "Lewis", Order = 3,
                TranslationKey = "letter.lewis_01",
                LocationType = SpawnLocationType.Trash, SeasonHint = "spring"
            },

            // Summer — Life in the valley
            new LetterData
            {
                Id = "grandpa_03", Author = "Grandpa", Order = 4,
                TranslationKey = "letter.grandpa_03",
                LocationType = SpawnLocationType.Beach, SeasonHint = "summer"
            },
            new LetterData
            {
                Id = "grandpa_04", Author = "Grandpa", Order = 5,
                TranslationKey = "letter.grandpa_04",
                LocationType = SpawnLocationType.Fishing, SeasonHint = "summer"
            },
            new LetterData
            {
                Id = "grandpa_lewis", Author = "Grandpa", Order = 6,
                TranslationKey = "letter.grandpa_lewis",
                LocationType = SpawnLocationType.Trash, SeasonHint = "summer"
            },

            // Fall — Golden days
            new LetterData
            {
                Id = "grandpa_05", Author = "Grandpa", Order = 7,
                TranslationKey = "letter.grandpa_05",
                LocationType = SpawnLocationType.Farm, SeasonHint = "fall"
            },
            new LetterData
            {
                Id = "grandpa_06", Author = "Grandpa", Order = 8,
                TranslationKey = "letter.grandpa_06",
                LocationType = SpawnLocationType.Mines, SeasonHint = "fall"
            },
            new LetterData
            {
                Id = "evelyn_01", Author = "Evelyn", Order = 9,
                TranslationKey = "letter.evelyn_01",
                LocationType = SpawnLocationType.Beach, SeasonHint = "fall"
            },

            // Winter — Reflection
            new LetterData
            {
                Id = "grandpa_07", Author = "Grandpa", Order = 10,
                TranslationKey = "letter.grandpa_07",
                LocationType = SpawnLocationType.Farm, SeasonHint = "winter"
            },
            new LetterData
            {
                Id = "grandpa_08", Author = "Grandpa", Order = 11,
                TranslationKey = "letter.grandpa_08",
                LocationType = SpawnLocationType.Mines, SeasonHint = "winter"
            },
            new LetterData
            {
                Id = "oldfisherman_01", Author = "Old Fisherman", Order = 12,
                TranslationKey = "letter.oldfisherman_01",
                LocationType = SpawnLocationType.Fishing, SeasonHint = "winter"
            },

            // Special — Any season
            new LetterData
            {
                Id = "grandpa_09", Author = "Grandpa", Order = 13,
                TranslationKey = "letter.grandpa_09",
                LocationType = SpawnLocationType.Farm, SeasonHint = null
            },
            new LetterData
            {
                Id = "grandpa_10", Author = "Grandpa", Order = 14,
                TranslationKey = "letter.grandpa_10",
                LocationType = SpawnLocationType.Farm, SeasonHint = null
            },
            new LetterData
            {
                Id = "mystery_01", Author = "???", Order = 15,
                TranslationKey = "letter.mystery_01",
                LocationType = SpawnLocationType.Mines, SeasonHint = null
            }
        };
    }
}
