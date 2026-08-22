using System.Linq;
using StardewModdingAPI;
using StardewValley;
using Object = StardewValley.Object;

namespace LMQoL.Features.UnstickItem
{
    /// <summary>Frees an item the game won't let go of.
    ///
    /// Quest and special items are flagged so they can't be trashed or sold — sensible while the
    /// quest needs them, but a leftover copy (an extra drop, or one kept after the order ended)
    /// then occupies a slot forever. This clears those flags on the held item so it behaves like
    /// any other, and can optionally delete it outright.</summary>
    public class UnstickItemFeature : IFeature
    {
        public string Id => "UnstickItem";

        public void Register(IModHelper helper, IMonitor monitor)
        {
            helper.ConsoleCommands.Add("lmqol_unstick",
                "Make the item you're holding trashable/sellable again (clears quest and special flags).\n"
                + "Usage: lmqol_unstick        - unstick the item in your active slot\n"
                + "       lmqol_unstick all    - unstick every item in your inventory\n"
                + "       lmqol_unstick delete - delete the item in your active slot outright",
                (_, args) => Run(args, monitor));
        }

        private static void Run(string[] args, IMonitor log)
        {
            if (!Context.IsWorldReady)
            {
                log.Log("Load a save first.", LogLevel.Warn);
                return;
            }

            var player = Game1.player;
            string mode = args.FirstOrDefault()?.ToLowerInvariant() ?? "";

            if (mode == "all")
            {
                int freed = player.Items.Count(item => item != null && Unstick(item));
                log.Log(freed > 0 ? $"Unstuck {freed} item(s)." : "Nothing was stuck.", LogLevel.Info);
                return;
            }

            var held = player.CurrentItem;
            if (held == null)
            {
                log.Log("Select the item in your toolbar first (it must be the active slot).", LogLevel.Warn);
                return;
            }

            if (mode == "delete")
            {
                player.removeItemFromInventory(held);
                log.Log($"Deleted {held.DisplayName}.", LogLevel.Info);
                return;
            }

            log.Log(Unstick(held)
                ? $"{held.DisplayName} can now be trashed or sold."
                : $"{held.DisplayName} wasn't flagged as stuck; it should already be trashable.",
                LogLevel.Info);
        }

        /// <summary>Clear the flags that block trashing. Returns true if anything changed.</summary>
        private static bool Unstick(Item item)
        {
            bool changed = false;

            if (item.specialItem)
            {
                item.specialItem = false;
                changed = true;
            }

            if (item is Object obj && obj.questItem.Value)
            {
                obj.questItem.Value = false;
                changed = true;
            }

            return changed;
        }
    }
}
