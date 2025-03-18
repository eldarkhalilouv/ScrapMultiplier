using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using ScrapMultiplier.Config;
using ScrapMultiplier.Patches;
using PluginInfo = ScrapMultiplier.Properties.PluginInfo;

namespace ScrapMultiplier;

[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    public static ManualLogSource Logger = BepInEx.Logging.Logger.CreateLogSource("ScrapMultiplier");
    private readonly Harmony _harmony = new(PluginInfo.PLUGIN_GUID);

    private void Awake()
    {
        Logger = base.Logger;
        ConfigManager.Init(Config);
        _harmony.PatchAll(typeof(ScrapPatches));
        _harmony.PatchAll(typeof(ConfigPatches));
        Logger.LogInfo($"{PluginInfo.PLUGIN_NAME} loaded!");
    }
}