using System.Collections.Generic;
using System.Linq;
using System.Text;
using NPCMemory.Models;
using StardewModdingAPI;
using StardewValley;

namespace NPCMemory.Data
{
    /// <summary>
    /// Builds the text of the weekly "Valley Gazette" newsletter by aggregating
    /// the last several recorded days of activity and attributing each headline
    /// to the town NPC whose "beat" matches the activity.
    /// </summary>
    public class NewsletterGenerator
    {
        private readonly IModHelper _helper;
        private readonly MemoryStore _memoryStore;
        private readonly ModConfig _config;

        // The town NPC who "reports" on each kind of activity.
        private static readonly Dictionary<ActivityType, string> Reporters = new()
        {
            [ActivityType.Fishing] = "Willy",
            [ActivityType.Mining] = "Clint",
            [ActivityType.Farming] = "Pierre",
            [ActivityType.Cooking] = "Gus",
            [ActivityType.Combat] = "Kent",
            [ActivityType.Foraging] = "Linus",
            [ActivityType.AnimalCare] = "Marnie",
            [ActivityType.ArtisanGoods] = "Robin",
            [ActivityType.Flowers] = "Evelyn",
            [ActivityType.Beverages] = "Pam",
            [ActivityType.Sweets] = "Haley",
            [ActivityType.Technology] = "Maru",
            [ActivityType.VoidItems] = "Krobus",
            [ActivityType.Magic] = "Wizard",
        };

        public NewsletterGenerator(IModHelper helper, MemoryStore memoryStore, ModConfig config)
        {
            _helper = helper;
            _memoryStore = memoryStore;
            _config = config;
        }

        /// <summary>Builds the newsletter body, or null if there's nothing worth reporting.</summary>
        public string? Generate()
        {
            var history = _memoryStore.GetRecentDays(7);
            if (history.Count == 0)
                return null;

            // Aggregate the week's activity across all recorded days.
            var totals = new Dictionary<ActivityType, ActivityData>();
            foreach (var day in history)
            {
                foreach (var kvp in day.Activities)
                {
                    if (!totals.TryGetValue(kvp.Key, out var agg))
                    {
                        agg = new ActivityData();
                        totals[kvp.Key] = agg;
                    }

                    agg.Count += kvp.Value.Count;
                    foreach (var item in kvp.Value.NotableItems)
                    {
                        if (!agg.NotableItems.Contains(item) && agg.NotableItems.Count < 5)
                            agg.NotableItems.Add(item);
                    }
                }
            }

            var top = totals
                .Where(kv => kv.Value.Count > 0)
                .OrderByDescending(kv => kv.Value.Count)
                .Take(System.Math.Max(1, _config.NewsletterMaxHeadlines))
                .ToList();

            if (top.Count == 0)
                return null;

            var sb = new StringBuilder();
            sb.Append(T("newsletter.title"));
            sb.Append("^^");
            sb.Append(T("newsletter.intro"));
            sb.Append("^^");

            foreach (var kv in top)
            {
                sb.Append(BuildHeadline(kv.Key, kv.Value));
                sb.Append("^^");
            }

            sb.Append(T("newsletter.footer"));
            return sb.ToString();
        }

        private string BuildHeadline(ActivityType type, ActivityData data)
        {
            string key = $"newsletter.headline.{type.ToString().ToLower()}";
            string template = _helper.Translation.Get(key).ToString();

            string reporter = "";
            if (Reporters.TryGetValue(type, out var npcName))
                reporter = Game1.getCharacterFromName(npcName)?.displayName ?? npcName;

            string item = data.NotableItems.Count > 0 ? data.NotableItems[0] : "";
            string items = data.NotableItems.Count > 0 ? string.Join(", ", data.NotableItems) : "";

            return template
                .Replace("{reporter}", reporter)
                .Replace("{count}", data.Count.ToString())
                .Replace("{item}", item)
                .Replace("{items}", items);
        }

        private string T(string key) => _helper.Translation.Get(key).ToString();
    }
}
