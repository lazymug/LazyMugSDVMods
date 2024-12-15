using ElementalForce.Elemental_Force_Code.helpers;
using HarmonyLib;
using StardewValley;
using StardewValley.GameData.LocationContexts;
using StardewValley.Network;

namespace ElementalForce.Elemental_Force_Code.harmony;

[HarmonyPatch(typeof(LocationWeather))]
public static class LocationWeatherPatch
{
    [HarmonyPatch(nameof(LocationWeather.UpdateDailyWeather))]
    public static bool Prefix(LocationWeather __instance, string locationContextId, LocationContextData data, Random random)
    {
        if (!ToolAttachmentHelper.IsLeviathanShardEquipped()) return true;
        __instance.WeatherForTomorrow = "Storm";
        return false;
    }
}