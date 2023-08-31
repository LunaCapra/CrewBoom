using BepInEx;
using BepInEx.Logging;
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

            Logger.LogInfo("Loading character assets...");
            CustomAssets.Initialize(Paths.PluginPath);
            Logger.LogInfo("Character assets loaded.");

            Harmony harmony = new Harmony("io.sgiygas.brcCustomCharacters");
            harmony.PatchAll();
        }
    }
}
