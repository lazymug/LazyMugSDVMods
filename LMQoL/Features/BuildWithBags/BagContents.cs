using System.Collections.Generic;
using StardewValley;
using Object = StardewValley.Object;

namespace LMQoL.Features.BuildWithBags
{
    /// <summary>Counting and withdrawing build materials stored inside Item Bags.</summary>
    internal static class BagContents
    {
        /// <summary>How many of <paramref name="qualifiedId"/> sit inside the player's bags.</summary>
        public static int Count(IItemBagsApi api, Farmer player, string qualifiedId)
        {
            int total = 0;
            foreach (var bag in api.GetItemBags(player.Items))
            {
                foreach (var obj in api.GetObjectsInsideBag(bag, includeNestedBags: true))
                {
                    if (obj?.QualifiedItemId == qualifiedId)
                        total += obj.Stack;
                }
            }
            return total;
        }

        /// <summary>Take up to <paramref name="amount"/> of an item out of the player's bags.
        /// Returns how many were actually taken.</summary>
        /// <remarks>The withdrawn items are dropped into a throwaway container: the caller is
        /// consuming them as build materials, and routing them through the real inventory would
        /// fail whenever the player has no free slot.</remarks>
        public static int Take(IItemBagsApi api, Farmer player, string qualifiedId, int amount)
        {
            if (amount <= 0)
                return 0;

            int taken = 0;
            var sink = new List<Item>();

            foreach (var bag in api.GetItemBags(player.Items))
            {
                if (taken >= amount)
                    break;

                // snapshot: removing mutates the bag's contents
                var matches = new List<Object>();
                foreach (var obj in api.GetObjectsInsideBag(bag, includeNestedBags: true))
                {
                    if (obj?.QualifiedItemId == qualifiedId)
                        matches.Add(obj);
                }

                foreach (var match in matches)
                {
                    if (taken >= amount)
                        break;

                    int want = System.Math.Min(amount - taken, match.Stack);
                    if (api.TryRemoveObjectFromBag(bag, match, want, sink, int.MaxValue,
                            playSoundEffect: false, out int moved))
                        taken += moved;

                    sink.Clear();
                }
            }

            return taken;
        }
    }
}
