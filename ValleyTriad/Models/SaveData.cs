using System.Collections.Generic;

namespace ValleyTriad.Models
{
    /// <summary>Per-save persisted state: owned cards (count per id), the chosen deck, and flags.</summary>
    public class SaveData
    {
        public Dictionary<string, int> Owned { get; set; } = new();
        public List<string> Deck { get; set; } = new();
        public bool IntroSeen { get; set; }
    }
}
