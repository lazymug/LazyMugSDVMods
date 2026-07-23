using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI;
using ValleyTriad.Models;

namespace ValleyTriad.Data
{
    /// <summary>Serializable card record (assets/cards.json). Enums parse from their names.</summary>
    public class CardData
    {
        public string Id { get; set; } = "";
        public string NameKey { get; set; } = "";
        public Tier Tier { get; set; }
        public Season Element { get; set; }
        public Category Category { get; set; }
        public string Sprite { get; set; } = "";
        public int SpriteIndex { get; set; }
        public int N { get; set; }
        public int E { get; set; }
        public int S { get; set; }
        public int W { get; set; }
    }

    /// <summary>Loads and indexes the card roster from assets/cards.json.</summary>
    public class CardDatabase
    {
        private readonly Dictionary<string, Card> _byId = new();
        public IReadOnlyCollection<Card> All => _byId.Values;

        public void Load(IModHelper helper, IMonitor monitor)
        {
            _byId.Clear();
            var data = helper.Data.ReadJsonFile<List<CardData>>("assets/cards.json") ?? new();
            foreach (var d in data)
            {
                _byId[d.Id] = new Card(d.Id, string.IsNullOrEmpty(d.NameKey) ? d.Id : d.NameKey,
                    d.Tier, d.Element, d.Category, d.Sprite, d.SpriteIndex, d.N, d.E, d.S, d.W);
            }
            monitor.Log($"Loaded {_byId.Count} cards from assets/cards.json.", LogLevel.Info);
        }

        public Card? Get(string id) => _byId.TryGetValue(id, out var c) ? c : null;

        public IEnumerable<Card> ByTier(Tier tier) => _byId.Values.Where(c => c.Tier == tier);
    }
}
