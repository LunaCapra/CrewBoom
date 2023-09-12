using BepInEx;
using BepInEx.Logging;
using BrcCustomCharacters.Data;
using HarmonyLib;
using Reptile;
using System.Collections;
using UnityEngine;

namespace BrcCustomCharacters
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        private void Awake()
        {
            // Plugin startup logic
            Logger.LogInfo($"{PluginInfo.PLUGIN_GUID} loaded.");

            Logger.LogInfo("Loading configuration...");
            AssetConfig.Init(Config);
            Logger.LogInfo("Configuration loaded.");

            Logger.LogInfo("Initializing model replacement database.");
            AssetDatabase.Initialize(Paths.PluginPath);
            Logger.LogInfo("Database initialized.");

            Harmony harmony = new Harmony("io.sgiygas.brcCustomCharacters");
            harmony.PatchAll();
        }
    }
}
