using System.Collections.Generic;
using System.Linq;
using BepInEx.Configuration;
using Newtonsoft.Json;

namespace ScrapMultiplier.Config;

public class SyncedConfig
{
    public Dictionary<LevelWeatherType, float> ValueMultipliers = new();

    public SyncedConfig(ConfigFile config)
    {
        InitializeDefaults();
        LoadConfig(config);
    }

    private void InitializeDefaults()
    {
        ValueMultipliers = new Dictionary<LevelWeatherType, float>
        {
            {LevelWeatherType.Rainy, 1.05f},
            {LevelWeatherType.Stormy, 1.20f},
            {LevelWeatherType.Foggy, 1.15f},
            {LevelWeatherType.Flooded, 1.25f},
            {LevelWeatherType.Eclipsed, 1.50f}
        };
    }

    private void LoadConfig(ConfigFile config)
    {
        var weatherTypes = ValueMultipliers.Keys.ToList();
    
        foreach (var weather in weatherTypes)
        {
            ValueMultipliers[weather] = config.Bind(
                "Multipliers",
                weather.ToString(),
                ValueMultipliers[weather],
                $"Multiplier for {weather} weather"
            ).Value;
        }
    }

    public string ToJson() => JsonConvert.SerializeObject(this);
}