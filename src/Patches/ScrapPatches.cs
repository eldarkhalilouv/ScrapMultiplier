using System;
using HarmonyLib;
using ScrapMultiplier.Config;

namespace ScrapMultiplier.Patches;

[HarmonyPatch]
public static class ScrapPatches
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(RoundManager), "SpawnScrapInLevel")]
    public static void SpawnScrapPrefix(RoundManager __instance)
    {
        try
        {
            var weather = __instance.currentLevel.currentWeather;
            if (ConfigManager.Instance.ValueMultipliers.TryGetValue(weather, out var multiplier))
            {
                __instance.scrapValueMultiplier *= multiplier;
                Plugin.Logger.LogInfo($"Applied {multiplier}x multiplier for {weather}");
            }
        }
        catch (Exception e)
        {
            Plugin.Logger.LogError($"SpawnScrapPrefix error: {e}");
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(RoundManager), "SpawnScrapInLevel")]
    private static void SpawnScrapPostfix(RoundManager __instance)
    {
        try
        {
            var weather = __instance.currentLevel.currentWeather;
            if (ConfigManager.Instance.ValueMultipliers.TryGetValue(weather, out var multiplier))
            {
                __instance.scrapValueMultiplier /= multiplier;
            }
        }
        catch (Exception e)
        {
            Plugin.Logger.LogError($"SpawnScrapPostfix error: {e}");
        }
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(LungProp), "DisconnectFromMachinery")]
    private static void AdjustLungValue(LungProp __instance)
    {
        try
        {
            var weather = __instance.roundManager.currentLevel.currentWeather;
            if (ConfigManager.Instance.ValueMultipliers.TryGetValue(weather, out var multiplier))
            {
                __instance.scrapValue = (int)(__instance.scrapValue * multiplier);
                Plugin.Logger.LogInfo($"Adjusted LungProp value to {__instance.scrapValue}");
            }
        }
        catch (Exception e)
        {
            Plugin.Logger.LogError($"LungProp error: {e}"); 
        }
    }
}