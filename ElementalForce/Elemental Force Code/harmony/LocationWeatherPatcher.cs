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
        {
            return;
        }
        var updateWeatherChance = 0;
        if (Game1.player.buffs.IsApplied(BuffHelper.GetBuffRainWishId()))
        {
            updateWeatherChance += 50;
        }
        if (Game1.player.buffs.IsApplied(BuffHelper.GetBuffThunderCallerId()))
        {
            updateWeatherChance += 40;
        }
        var chance = random.Next(0, 100);
        if (chance == 40 && updateWeatherChance < chance)
        {
            __instance.WeatherForTomorrow = "Storm";
        }
        if (chance == 50 && updateWeatherChance < chance)
        {
            __instance.WeatherForTomorrow = "Rain";
        }
        if (chance == 90 && updateWeatherChance >= chance)
        {
            return;
        }

        if (chance < 60)
        {
            __instance.WeatherForTomorrow = "Rain";
        }
        else
        {
            __instance.WeatherForTomorrow = "Storm";
        }
    }
}