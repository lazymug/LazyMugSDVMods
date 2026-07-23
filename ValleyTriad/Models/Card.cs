namespace ValleyTriad.Models
{
    /// <summary>
    /// A card definition (immutable). Art is composed at runtime from <see cref="Sprite"/>;
    /// no per-card image is shipped. Edges are the four Triple Triad values (1–10).
    /// </summary>
    public class Card
    {
        public string Id { get; }
        /// <summary>i18n/display key or fallback name. Object cards resolve their localized name at draw time.</summary>
        public string NameKey { get; }
        public Tier Tier { get; }
        public Season Element { get; }
        public Category Category { get; }
        /// <summary>Sprite source: "(O)&lt;id&gt;" object, "Villager:&lt;Name&gt;", "Animal:&lt;Type&gt;", "Monster:&lt;Type&gt;", "Special:&lt;Name&gt;".</summary>
        public string Sprite { get; }
        public int SpriteIndex { get; }

        private readonly int[] _edges; // N, E, S, W

        public Card(string id, string nameKey, Tier tier, Season element, Category category,
                    string sprite, int spriteIndex, int n, int e, int s, int w)
        {
            Id = id; NameKey = nameKey; Tier = tier; Element = element; Category = category;
            Sprite = sprite; SpriteIndex = spriteIndex;
            _edges = new[] { n, e, s, w };
        }

        public int Edge(Dir d) => _edges[(int)d];

        public int EdgeSum() => _edges[0] + _edges[1] + _edges[2] + _edges[3];

        public override string ToString() => $"{Id}({_edges[0]}/{_edges[1]}/{_edges[2]}/{_edges[3]})";
    }
}
