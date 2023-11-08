using BepInEx;
using HarmonyLib;

namespace CrewBoom
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        private void Awake()
        {
            Logger.LogMessage($"{PluginInfo.PLUGIN_GUID} v{PluginInfo.PLUGIN_VERSION} starting...");

            CharacterDatabaseConfig.Initialize(Config);

            if (CharacterDatabase.Initialize())
            {
                Harmony harmony = new Harmony("sgiygas.crewBoom");
                harmony.PatchAll();

                Logger.LogMessage($"Loaded all available characters!");
            }
        }
    }
}
