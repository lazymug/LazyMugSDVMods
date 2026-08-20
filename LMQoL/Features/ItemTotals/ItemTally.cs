using System.Collections.Generic;
using LMQoL.Features.BuildWithBags;
using StardewValley;
using StardewValley.Objects;

namespace LMQoL.Features.ItemTotals
{
    /// <summary>Where the copies of an item are and how many there are.</summary>
    internal readonly struct ItemTally
    {
        public readonly int Carried;
        public readonly int InChests;
        public readonly int InBags;

        public ItemTally(int carried, int inChests, int inBags)
        {
            Carried = carried;
            InChests = inChests;
            InBags = inBags;
        }

        public int Total => Carried + InChests + InBags;

        /// <summary>Walk the world once and add up every copy of <paramref name="qualifiedId"/>.</summary>
        /// <remarks>Counting chests means visiting every location, so callers should cache the
        /// result per hovered item rather than calling this each frame.</remarks>
        public static ItemTally Count(string qualifiedId, bool includeChests, bool includeBags)
        {
            var player = Game1.player;
            int carried = CountIn(player.Items, qualifiedId);
            int chests = 0;
            int bags = 0;

            if (includeChests)
            {
                var seen = new HashSet<Chest>();
                Utility.ForEachLocation(location =>
                {
                    foreach (var obj in location.Objects.Values)
                    {
                        if (obj is Chest chest && seen.Add(chest))
                            chests += CountIn(chest.Items, qualifiedId);
                    }

                    // fridges and other stashes that aren't placed objects
                    foreach (var chest in location.buildings.Count > 0 ? BuildingChests(location) : NoChests)
                    {
                        if (seen.Add(chest))
                            chests += CountIn(chest.Items, qualifiedId);
                    }

                    if (location is StardewValley.Locations.FarmHouse house && house.fridge.Value is Chest fridge && seen.Add(fridge))
                        chests += CountIn(fridge.Items, qualifiedId);
                    if (location is StardewValley.Locations.IslandFarmHouse island && island.fridge.Value is Chest islandFridge && seen.Add(islandFridge))
                        chests += CountIn(islandFridge.Items, qualifiedId);

                    return true;
                });
            }

            if (includeBags && BuildWithBagsFeature.Api is { } api)
                bags = BagContents.Count(api, player, qualifiedId);

            return new ItemTally(carried, chests, bags);
        }

        private static readonly Chest[] NoChests = new Chest[0];

        private static IEnumerable<Chest> BuildingChests(GameLocation location)
        {
            foreach (var building in location.buildings)
            {
                foreach (var chest in building.buildingChests)
                    yield return chest;
            }
        }

        private static int CountIn(IList<Item> items, string qualifiedId)
        {
            int total = 0;
            foreach (var item in items)
            {
                if (item?.QualifiedItemId == qualifiedId)
                    total += item.Stack;
            }
            return total;
        }
    }
}
