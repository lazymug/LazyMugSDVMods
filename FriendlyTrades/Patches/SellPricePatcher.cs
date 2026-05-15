using FriendlyTrades.Utils;
using HarmonyLib;

namespace FriendlyTrades.Patches
{
    [HarmonyPatch(typeof(StardewValley.Object), nameof(StardewValley.Object.sellToStorePrice))]
    public static class SellPricePatcher
    {
        public static void Postfix(StardewValley.Object __instance, ref int __result)
        {
            if (ModEntry.CurrentSpecialty == null || ModEntry.CurrentNpcName == null)
                return;

            if (ModEntry.CurrentSpecialty.AcceptsItem(__instance))
            {
                float bonus = BonusCalculator.GetBonus(ModEntry.CurrentNpcName);
                float multiplier = ModEntry.Config.BonusMultiplier;
                __result = (int)(__result * (1f + bonus * multiplier));
            }
            else if (ModEntry.IsCustomTradeShop)
            {
                __result = 0;
            }
        }
    }
}
