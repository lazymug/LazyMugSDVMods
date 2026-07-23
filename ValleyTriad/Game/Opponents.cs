using System.Collections.Generic;
using System.Linq;
using StardewValley;
using ValleyTriad.Data;
using ValleyTriad.Models;

namespace ValleyTriad.Game
{
    /// <summary>Builds opponent decks and AI skill scaled by friendship, plus the starter pack.</summary>
    public static class Opponents
    {
        /// <summary>Cards granted by Abigail's intro event.</summary>
        public static readonly string[] StarterPack =
            { "parsnip", "cauliflower", "pumpkin", "chicken", "abigail", "diamond" };

        /// <summary>AI skill 0–2 (0 plays loosely, 2 is fully greedy).</summary>
        public static int AiSkill(int hearts) => hearts >= 8 ? 2 : hearts >= 4 ? 1 : 0;

        /// <summary>Picks a 5-card opponent deck; stronger tiers unlock with friendship. Respects deck caps.</summary>
        public static List<Card> BuildDeck(CardDatabase db, int hearts)
        {
            // tier counts by heart level
            (int com, int unc, int rar, int leg) plan =
                hearts >= 10 ? (1, 2, 1, 1) :
                hearts >= 8 ? (2, 2, 1, 0) :
                hearts >= 5 ? (3, 2, 0, 0) :
                hearts >= 2 ? (4, 1, 0, 0) :
                (5, 0, 0, 0);

            var rng = Game1.random;
            var deck = new List<Card>();
            void Take(Tier tier, int n)
            {
                var pool = db.ByTier(tier).ToList();
                for (int i = 0; i < n && pool.Count > 0; i++)
                    deck.Add(pool[rng.Next(pool.Count)]);
            }
            Take(Tier.Legendary, plan.leg);
            Take(Tier.Rare, plan.rar);
            Take(Tier.Uncommon, plan.unc);
            Take(Tier.Common, plan.com);

            // pad to 5 from any tier if some pools were empty
            var all = db.All.ToList();
            while (deck.Count < Deck.Size && all.Count > 0) deck.Add(all[rng.Next(all.Count)]);
            return deck.Take(Deck.Size).ToList();
        }
    }
}
