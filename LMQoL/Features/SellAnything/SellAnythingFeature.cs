using StardewModdingAPI;
using StardewValley;
using Object = StardewValley.Object;

namespace LMQoL.Features.SellAnything
{
    /// <summary>Lets items the game normally refuses to sell — furniture, wallpaper, big
    /// craftables — go into the shipping bin, and optionally be sold in shops.</summary>
    public class SellAnythingFeature : IFeature
    {
        public string Id => "SellAnything";

        public void Register(IModHelper helper, IMonitor monitor)
        {
            // behaviour lives entirely in the Harmony patches
        }

        /// <summary>Whether we're willing to unlock this item, keeping the game's own safety rails:
        /// anything that can't be trashed (tools, the scythe, special items) and quest items stay
        /// unsellable, so nothing important can be shipped away by accident.</summary>
        internal static bool IsSafeToSell(Item? item)
        {
            if (item == null || !ModEntry.Config.SellAnythingEnabled)
                return false;

            if (!item.canBeTrashed())
                return false;

            if (item is Object obj && obj.questItem.Value)
                return false;

            return true;
        }
    }
}
