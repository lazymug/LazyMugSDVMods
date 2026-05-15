using System.Collections.Generic;
using System.Linq;
using NPCMemory.Models;
using StardewModdingAPI;

namespace NPCMemory.Data
{
    public class MemoryStore
    {
        private const int MaxDaysKept = 7;

        private readonly IModHelper _helper;
        private List<DailyActivity> _history = new();

        public MemoryStore(IModHelper helper)
        {
            _helper = helper;
        }

        public void Load()
        {
            _history = _helper.Data.ReadSaveData<List<DailyActivity>>("NPCMemory.History") ?? new();
        }

        public void Save()
        {
            _helper.Data.WriteSaveData("NPCMemory.History", _history);
        }

        public void PushDay(DailyActivity day)
        {
            // Only store days where something happened
            if (day.Activities.Count == 0)
                return;

            _history.Add(day);

            // Keep only the last N days
            if (_history.Count > MaxDaysKept)
                _history = _history.Skip(_history.Count - MaxDaysKept).ToList();
        }

        public DailyActivity? GetYesterday()
        {
            return _history.Count > 0 ? _history[^1] : null;
        }

        public List<DailyActivity> GetRecentDays(int count = 3)
        {
            int take = System.Math.Min(count, _history.Count);
            return _history.Skip(_history.Count - take).ToList();
        }
    }
}
