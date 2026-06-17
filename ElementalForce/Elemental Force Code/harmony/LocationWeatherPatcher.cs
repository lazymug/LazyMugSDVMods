using ElementalForce.Elemental_Force_Code.helpers;
using HarmonyLib;
using StardewValley;
using StardewValley.GameData.LocationContexts;
using StardewValley.Network;

namespace ElementalForce.Elemental_Force_Code.harmony;

[HarmonyPatch(typeof(LocationWeather))]
public static class LocationWeatherPatcher
{
    [HarmonyPatch(nameof(LocationWeather.UpdateDailyWeather))]
    public static void Postfix(LocationWeather __instance, string locationContextId, LocationContextData data, Random random)
    {
        if (__instance.WeatherForTomorrow == "Festival")
            return;

        var hasRainWish = Game1.player.buffs.IsApplied(BuffHelper.GetBuffRainWishId());
        var hasThunderCaller = Game1.player.buffs.IsApplied(BuffHelper.GetBuffThunderCallerId());

        if (!hasRainWish && !hasThunderCaller)
            return;

        var totalChance = 0;
        if (hasRainWish)
            totalChance += BuffConstants.RainWishWeatherChanceBonus;
        if (hasThunderCaller)
            totalChance += BuffConstants.ThunderCallerWeatherChanceBonus;

        var roll = random.Next(0, 100);
        if (roll >= totalChance)
            return;

        if (hasThunderCaller && roll < BuffConstants.ThunderCallerWeatherChanceBonus)
            __instance.WeatherForTomorrow = "Storm";
        else
            __instance.WeatherForTomorrow = "Rain";
    }
}
