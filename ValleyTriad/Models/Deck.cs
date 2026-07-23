using System.Collections.Generic;
using System.Linq;

namespace ValleyTriad.Models
{
    /// <summary>A hand/deck of exactly five cards taken to a match.</summary>
    public class Deck
    {
        public const int Size = 5;
        public const int MaxLegendary = 1;
        public const int MaxRare = 2;

        public List<Card> Cards { get; } = new();

        public bool IsValid(out string? error)
        {
            if (Cards.Count != Size) { error = $"A deck must have exactly {Size} cards."; return false; }
            if (Cards.Count(c => c.Tier == Tier.Legendary) > MaxLegendary) { error = $"At most {MaxLegendary} Legendary card."; return false; }
            if (Cards.Count(c => c.Tier == Tier.Rare) > MaxRare) { error = $"At most {MaxRare} Rare cards."; return false; }
            error = null; return true;
        }
    }
}
