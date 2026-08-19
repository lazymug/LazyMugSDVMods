using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace LMQoL.Features.BuildWithBags
{
    /// <summary>Lets Robin count build materials stored inside Item Bags.
    ///
    /// Entirely opt-in on Item Bags being installed: without it the API is null and every patch
    /// falls straight through to vanilla behaviour.</summary>
    public class BuildWithBagsFeature : IFeature
    {
        private const string ItemBagsId = "SlayerDharok.Item_Bags";

        internal static IItemBagsApi? Api { get; private set; }
        internal static IMonitor? Log { get; private set; }

        /// <summary>True when the feature should act: enabled in config and Item Bags present.</summary>
        internal static bool Active => Api != null && ModEntry.Config.BuildWithBagsEnabled;

        public string Id => "BuildWithBags";

        public void Register(IModHelper helper, IMonitor monitor)
        {
            Log = monitor;
            helper.Events.GameLoop.GameLaunched += (_, _) => Hook(helper, monitor);
        }

        private static void Hook(IModHelper helper, IMonitor monitor)
        {
            if (!helper.ModRegistry.IsLoaded(ItemBagsId))
                return;

            Api = helper.ModRegistry.GetApi<IItemBagsApi>(ItemBagsId);
            if (Api == null)
                monitor.Log("Item Bags is installed but its API could not be read; building from bags is off.", LogLevel.Warn);
            else
                monitor.Log("Item Bags detected — Robin will count materials stored in bags.", LogLevel.Debug);
        }
    }
}
