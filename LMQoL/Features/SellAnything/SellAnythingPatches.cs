using HarmonyLib;
using StardewValley;
using StardewValley.Menus;
using Object = StardewValley.Object;

namespace LMQoL.Features.SellAnything
{
    /// <summary>Everything that blocks selling funnels through two checks: canBeShipped() for the
    /// shipping bin (both the click-with-item path and the grab menu's highlight), and
    /// ShopMenu.highlightItemToSell for shops, which is what the inventory menu uses to decide
    /// whether an item can be picked up for sale.</summary>
    internal static class SellAnythingPatches
    {
        /// <summary>Objects: the base rule rejects furniture, wallpaper and big craftables.</summary>
        [HarmonyPatch(typeof(Object), nameof(Object.canBeShipped))]
        internal static class ObjectCanBeShipped
        {
            internal static void Postfix(Object __instance, ref bool __result)
            {
                if (__result || !ModEntry.Config.SellAnythingShipping)
                    return;

                if (SellAnythingFeature.IsSafeToSell(__instance))
                    __result = true;
            }
        }

        /// <summary>Non-object items (hats, boots, clothing…) fall through to Item.canBeShipped,
        /// which always returns false.</summary>
        [HarmonyPatch(typeof(Item), nameof(Item.canBeShipped))]
        internal static class ItemCanBeShipped
        {
            internal static void Postfix(Item __instance, ref bool __result)
            {
                if (__result || !ModEntry.Config.SellAnythingShipping)
                    return;

                if (SellAnythingFeature.IsSafeToSell(__instance))
                    __result = true;
            }
        }

        /// <summary>Shops only buy the categories and tags they're configured for.</summary>
        [HarmonyPatch(typeof(ShopMenu), nameof(ShopMenu.highlightItemToSell))]
        internal static class ShopHighlightItemToSell
        {
            internal static void Postfix(ShopMenu __instance, Item i, ref bool __result)
            {
                if (__result || !ModEntry.Config.SellAnythingShops)
                    return;

                // while holding an item the vanilla branch is about stacking, not eligibility
                if (__instance.heldItem != null)
                    return;

                if (SellAnythingFeature.IsSafeToSell(i))
                    __result = true;
            }
        }
    }
}
