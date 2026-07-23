namespace ValleyTriad.Models
{
    /// <summary>Rarity tier — drives the frame inlay, edge budget and deck caps.</summary>
    public enum Tier { Common, Uncommon, Rare, Legendary }

    /// <summary>The launch element system. Spirits (Elemental Force) are a deferred, separate feature.</summary>
    public enum Season { None, Spring, Summer, Fall, Winter }

    /// <summary>Card subject family — selects the themed background scene.</summary>
    public enum Category { Crop, Forage, Animal, Fish, Monster, Mineral, Villager, Special }

    /// <summary>Card edge, in reading order N, E, S, W.</summary>
    public enum Dir { N = 0, E = 1, S = 2, W = 3 }

    public static class DirExtensions
    {
        public static Dir Opposite(this Dir d) => d switch
        {
            Dir.N => Dir.S,
            Dir.S => Dir.N,
            Dir.E => Dir.W,
            _ => Dir.E,
        };

        /// <summary>Row/column delta for a direction (row grows downward = South).</summary>
        public static (int dr, int dc) Delta(this Dir d) => d switch
        {
            Dir.N => (-1, 0),
            Dir.S => (1, 0),
            Dir.E => (0, 1),
            _ => (0, -1),
        };
    }
}
