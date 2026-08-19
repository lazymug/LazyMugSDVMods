using System;
using HarmonyLib;
using StardewValley;
using StardewValley.Inventories;
using StardewValley.Menus;

namespace LMQoL.Features.BuildWithBags
{
    /// <summary>Makes the carpenter menu see (and spend) materials kept in Item Bags.</summary>
    internal static class CarpenterBagPatches
    {
        /// <summary>Only meddle while Robin's (or the Wizard's) build menu is the active menu, and
        /// only for the player's own inventory — this method is called all over the game.</summary>
        private static bool InBuildMenu(IInventory inventory)
            => BuildWithBagsFeature.Active
               && Game1.activeClickableMenu is CarpenterMenu
               && Game1.player != null
               && ReferenceEquals(inventory, Game1.player.Items);

        /// <summary>The build menu asks the inventory whether it holds enough of each material —
        /// both to enable the button and to colour the ingredient list. Top the count up with
        /// whatever is inside the player's bags.</summary>
        [HarmonyPatch(typeof(Inventory), nameof(Inventory.ContainsId), new[] { typeof(string), typeof(int) })]
        internal static class ContainsIdPatch
        {
            internal static void Postfix(Inventory __instance, string itemId, int minimum, ref bool __result)
            {
                if (__result || !InBuildMenu(__instance))
                    return;

                try
                {
                    int inInventory = __instance.CountId(itemId);
                    if (inInventory >= minimum)
                        return;

                    int inBags = BagContents.Count(BuildWithBagsFeature.Api!, Game1.player, itemId);
                    if (inInventory + inBags >= minimum)
                        __result = true;
                }
                catch (Exception ex)
                {
                    BuildWithBagsFeature.Log?.LogOnce($"Could not read Item Bags contents: {ex.Message}", StardewModdingAPI.LogLevel.Warn);
                }
            }
        }

        /// <summary>Pay for the build: take from the bags whatever the inventory is short of,
        /// before vanilla reduces the inventory itself.</summary>
        [HarmonyPatch(typeof(CarpenterMenu), nameof(CarpenterMenu.ConsumeResources))]
        internal static class ConsumeResourcesPatch
        {
            internal static void Prefix(CarpenterMenu __instance)
            {
                if (!BuildWithBagsFeature.Active)
                    return;

                try
                {
                    var player = Game1.player;
                    foreach (var ingredient in __instance.ingredients)
                    {
                        int missing = ingredient.Stack - player.Items.CountId(ingredient.QualifiedItemId);
                        if (missing > 0)
                            BagContents.Take(BuildWithBagsFeature.Api!, player, ingredient.QualifiedItemId, missing);
                    }
                }
                catch (Exception ex)
                {
                    BuildWithBagsFeature.Log?.Log($"Could not spend Item Bags contents: {ex.Message}", StardewModdingAPI.LogLevel.Warn);
                }
            }
        }
    }
}
