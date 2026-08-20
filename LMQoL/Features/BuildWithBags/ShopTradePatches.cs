using System;
using HarmonyLib;
using StardewValley;
using StardewValley.Menus;

namespace LMQoL.Features.BuildWithBags
{
    /// <summary>Lets shops that trade goods for goods — the Desert Trader, the Island Trader and
    /// any shop with a TradeItem — see and spend items kept in Item Bags.
    ///
    /// Both sides funnel through two small methods on ShopMenu, so a postfix on the check and a
    /// prefix on the payment cover the whole flow.</summary>
    internal static class ShopTradePatches
    {
        /// <summary>Qi Gems and Golden Walnuts are counters on the world state, not inventory
        /// items, so bags can never hold them and the vanilla answer is already correct.</summary>
        private static bool IsRealItem(string qualifiedId)
            => qualifiedId != "(O)858" && qualifiedId != "(O)73";

        private static bool Active
            => BuildWithBagsFeature.Active && ModEntry.Config.ShopTradeWithBagsEnabled;

        /// <summary>The shop asks whether the player holds enough of the trade item, both to enable
        /// the purchase and to colour the requirement. Count what's in the bags too.</summary>
        [HarmonyPatch(typeof(ShopMenu), nameof(ShopMenu.HasTradeItem))]
        internal static class HasTradeItemPatch
        {
            internal static void Postfix(string itemId, int count, ref bool __result)
            {
                if (__result || !Active)
                    return;

                try
                {
                    string qualified = ItemRegistry.QualifyItemId(itemId);
                    if (qualified == null || !IsRealItem(qualified))
                        return;

                    int carried = Game1.player.Items.CountId(qualified);
                    if (carried >= count)
                        return;

                    int inBags = BagContents.Count(BuildWithBagsFeature.Api!, Game1.player, qualified);
                    if (carried + inBags >= count)
                        __result = true;
                }
                catch (Exception ex)
                {
                    BuildWithBagsFeature.Log?.LogOnce($"Could not read Item Bags contents for a trade: {ex.Message}", StardewModdingAPI.LogLevel.Warn);
                }
            }
        }

        /// <summary>Pay the trade: take from the bags whatever the inventory is short of, so the
        /// player doesn't have to pull items out by hand first.</summary>
        [HarmonyPatch(typeof(ShopMenu), nameof(ShopMenu.ConsumeTradeItem))]
        internal static class ConsumeTradeItemPatch
        {
            internal static void Prefix(string itemId, int count)
            {
                if (!Active)
                    return;

                try
                {
                    string qualified = ItemRegistry.QualifyItemId(itemId);
                    if (qualified == null || !IsRealItem(qualified))
                        return;

                    int missing = count - Game1.player.Items.CountId(qualified);
                    if (missing > 0)
                        BagContents.Take(BuildWithBagsFeature.Api!, Game1.player, qualified, missing);
                }
                catch (Exception ex)
                {
                    BuildWithBagsFeature.Log?.Log($"Could not spend Item Bags contents for a trade: {ex.Message}", StardewModdingAPI.LogLevel.Warn);
                }
            }
        }
    }
}
