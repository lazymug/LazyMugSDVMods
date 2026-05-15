using System.Collections.Generic;
using System.Linq;
using NPCMemory.Models;
using StardewModdingAPI;
using StardewValley;

namespace NPCMemory.Data
{
    public class DialogueGenerator
    {
        private readonly IModHelper _helper;
        private readonly MemoryStore _memoryStore;

        public DialogueGenerator(IModHelper helper, MemoryStore memoryStore)
        {
            _helper = helper;
            _memoryStore = memoryStore;
        }

        public string? GenerateDialogue(string npcName)
        {
            var interests = NpcInterestRegistry.GetInterests(npcName);
            if (interests == null)
                return null;

            var yesterday = _memoryStore.GetYesterday();
            if (yesterday == null)
                return null;

            // Find the best matching activity: pick the one with highest count among NPC's interests
            ActivityType? bestMatch = null;
            ActivityData? bestData = null;

            foreach (var interest in interests)
            {
                if (yesterday.Activities.TryGetValue(interest, out var data))
                {
                    if (bestData == null || data.Count > bestData.Count)
                    {
                        bestMatch = interest;
                        bestData = data;
                    }
                }
            }

            if (bestMatch == null || bestData == null)
                return null;

            return BuildDialogueString(npcName, bestMatch.Value, bestData);
        }

        private string? BuildDialogueString(string npcName, ActivityType activity, ActivityData data)
        {
            string npcKey = npcName.ToLower();
            string activityKey = activity.ToString().ToLower();

            // Try NPC-specific dialogue first: "alex.fishing"
            string specificKey = $"dialogue.{npcKey}.{activityKey}";
            var specific = _helper.Translation.Get(specificKey);
            if (specific.HasValue())
                return FormatDialogue(specific.ToString(), data);

            // Fall back to generic activity dialogue: "dialogue.generic.fishing"
            string genericKey = $"dialogue.generic.{activityKey}";
            var generic = _helper.Translation.Get(genericKey);
            if (generic.HasValue())
                return FormatDialogue(generic.ToString(), data);

            return null;
        }

        private string FormatDialogue(string template, ActivityData data)
        {
            string itemName = data.NotableItems.Count > 0 ? data.NotableItems[0] : "";
            string itemList = data.NotableItems.Count > 0 ? string.Join(", ", data.NotableItems) : "";

            return template
                .Replace("{count}", data.Count.ToString())
                .Replace("{item}", itemName)
                .Replace("{items}", itemList);
        }
    }
}
