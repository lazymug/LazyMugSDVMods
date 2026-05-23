using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI;
using WeatheredLetters.Models;

namespace WeatheredLetters.Data
{
    public class SaveData
    {
        public HashSet<string> FoundLetterIds { get; set; } = new();
        public int LastLetterFoundDay { get; set; }
        public string? PendingLetterId { get; set; }
        public bool RewardGiven { get; set; }
    }

    public class CollectionTracker
    {
        private readonly IModHelper _helper;
        private SaveData _data = new();

        public CollectionTracker(IModHelper helper)
        {
            _helper = helper;
        }

        public void Load()
        {
            _data = _helper.Data.ReadSaveData<SaveData>("WeatheredLetters.Progress") ?? new SaveData();
        }

        public void Save()
        {
            _helper.Data.WriteSaveData("WeatheredLetters.Progress", _data);
        }

        public bool IsFound(string letterId) => _data.FoundLetterIds.Contains(letterId);

        public void MarkFound(string letterId, int currentDay)
        {
            _data.FoundLetterIds.Add(letterId);
            _data.LastLetterFoundDay = currentDay;
            _data.PendingLetterId = null;
            Save();
        }

        public int FoundCount => _data.FoundLetterIds.Count;
        public int TotalCount => LetterRegistry.AllLetters.Count;
        public bool AllFound => FoundCount >= TotalCount;
        public bool RewardGiven { get => _data.RewardGiven; set { _data.RewardGiven = value; Save(); } }
        public int LastLetterFoundDay => _data.LastLetterFoundDay;

        public string? PendingLetterId
        {
            get => _data.PendingLetterId;
            set { _data.PendingLetterId = value; Save(); }
        }

        public LetterData? GetNextLetter(string currentSeason)
        {
            // Prefer letters matching the current season, then any-season letters
            var unfound = LetterRegistry.AllLetters
                .Where(l => !IsFound(l.Id))
                .OrderBy(l => l.Order)
                .ToList();

            return unfound.FirstOrDefault(l => l.SeasonHint == currentSeason)
                   ?? unfound.FirstOrDefault(l => l.SeasonHint == null)
                   ?? unfound.FirstOrDefault();
        }
    }
}
