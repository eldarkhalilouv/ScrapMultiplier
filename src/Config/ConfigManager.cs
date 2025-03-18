using BepInEx.Configuration;

namespace ScrapMultiplier.Config;

public static class ConfigManager
{
    public static SyncedConfig Instance { get; private set; }

    public static void Init(ConfigFile config)
    {
        Instance = new SyncedConfig(config);
    }
}