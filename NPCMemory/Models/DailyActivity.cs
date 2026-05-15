using System.Collections.Generic;

namespace NPCMemory.Models
{
    public class DailyActivity
    {
        public int Day { get; set; }
        public string Season { get; set; } = "";
        public int Year { get; set; }

        public Dictionary<ActivityType, ActivityData> Activities { get; set; } = new();
    }

    public class ActivityData
    {
        public int Count { get; set; }
        public List<string> NotableItems { get; set; } = new();
    }
}
