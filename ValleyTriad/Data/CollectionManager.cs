using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI;
using ValleyTriad.Models;

namespace ValleyTriad.Data
{
    /// <summary>Owns the player's collection + deck and persists them to the current save.</summary>
    public class CollectionManager
    {
        private const string Key = "collection";
        private readonly IModHelper _helper;
        private readonly CardDatabase _db;
        private SaveData _data = new();

        public CollectionManager(IModHelper helper, CardDatabase db) { _helper = helper; _db = db; }

        public bool IntroSeen { get => _data.IntroSeen; set { _data.IntroSeen = value; Save(); } }

        public void Load() => _data = _helper.Data.ReadSaveData<SaveData>(Key) ?? new SaveData();
        public void Save() => _helper.Data.WriteSaveData(Key, _data);

        public int Count(string id) => _data.Owned.TryGetValue(id, out int n) ? n : 0;

        /// <summary>All owned cards expanded to Card objects (one entry per copy is not needed; use counts).</summary>
        public IEnumerable<(Card card, int count)> Owned()
            => _data.Owned.Where(kv => kv.Value > 0)
                          .Select(kv => (_db.Get(kv.Key), kv.Value))
                          .Where(t => t.Item1 != null)
                          .Select(t => (t.Item1!, t.Value));

        public void Add(string id, int n = 1)
        {
            _data.Owned[id] = Count(id) + n;
            Save();
        }

        public bool Remove(string id, int n = 1)
        {
            int have = Count(id);
            if (have <= 0) return false;
            _data.Owned[id] = System.Math.Max(0, have - n);
            if (_data.Owned[id] == 0) _data.Owned.Remove(id);
            // keep deck consistent if we no longer own a copy that the deck references beyond owned count
            TrimDeckToOwned();
            Save();
            return true;
        }

        public List<string> DeckIds => _data.Deck;

        public List<Card> DeckCards()
            => _data.Deck.Select(id => _db.Get(id)).Where(c => c != null).Select(c => c!).ToList();

        public void SetDeck(IEnumerable<string> ids) { _data.Deck = ids.ToList(); Save(); }

        /// <summary>The deck may reference at most the owned count of each card.</summary>
        private void TrimDeckToOwned()
        {
            var used = new Dictionary<string, int>();
            var trimmed = new List<string>();
            foreach (string id in _data.Deck)
            {
                used.TryGetValue(id, out int u);
                if (u < Count(id)) { trimmed.Add(id); used[id] = u + 1; }
            }
            _data.Deck = trimmed;
        }

        public bool HasValidDeck() => _data.Deck.Count == Deck.Size;

        /// <summary>If no valid deck is set, auto-pick a legal 5 (best edges first, respecting caps).</summary>
        public void EnsureDeck()
        {
            if (HasValidDeck()) return;
            var picks = new List<string>();
            int leg = 0, rar = 0;
            foreach (var (card, count) in Owned().OrderByDescending(t => t.card.EdgeSum()))
            {
                for (int k = 0; k < count && picks.Count < Deck.Size; k++)
                {
                    if (card.Tier == Tier.Legendary) { if (leg >= Deck.MaxLegendary) break; leg++; }
                    else if (card.Tier == Tier.Rare) { if (rar >= Deck.MaxRare) break; rar++; }
                    picks.Add(card.Id);
                }
                if (picks.Count >= Deck.Size) break;
            }
            if (picks.Count == Deck.Size) SetDeck(picks);
        }
    }
}
