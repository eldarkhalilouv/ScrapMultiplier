using System;
using GameNetcodeStuff;
using HarmonyLib;
using ScrapMultiplier.Config;
using ScrapMultiplier.Properties;
using Unity.Collections;
using Unity.Netcode;

namespace ScrapMultiplier.Patches;

[HarmonyPatch]
public static class ConfigPatches
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(PlayerControllerB), "ConnectClientToPlayerObject")]
    public static void InitializeSync()
    {
        if (NetworkManager.Singleton == null) return;

        if (NetworkManager.Singleton.IsServer)
        {
            NetworkManager.Singleton.CustomMessagingManager.RegisterNamedMessageHandler(
                $"{PluginInfo.PLUGIN_GUID}_ConfigSync",
                HandleConfigSyncRequest);
        }
        else if (NetworkManager.Singleton.IsClient)
        {
            RequestConfigSync();
        }
    }

    private static void RequestConfigSync()
    {
        using var writer = new FastBufferWriter(sizeof(int), Allocator.Temp);
        NetworkManager.Singleton.CustomMessagingManager.SendNamedMessage(
            $"{PluginInfo.PLUGIN_GUID}_ConfigSync",
            NetworkManager.ServerClientId,
            writer);
    }

    private static void HandleConfigSyncRequest(ulong clientId, FastBufferReader reader)
    {
        if (!NetworkManager.Singleton.IsServer) return;

        try
        {
            var configJson = ConfigManager.Instance.ToJson();
            var configData = System.Text.Encoding.UTF8.GetBytes(configJson);
                
            using var writer = new FastBufferWriter(configData.Length + sizeof(int), Allocator.Temp);
            writer.WriteValueSafe(configData.Length);
            writer.WriteBytesSafe(configData);

            NetworkManager.Singleton.CustomMessagingManager.SendNamedMessage(
                $"{PluginInfo.PLUGIN_GUID}_ConfigSync",
                clientId,
                writer
                );
        }
        catch (Exception e)
        {
            Plugin.Logger.LogError($"Config sync failed: {e}");
        } 
    }
}