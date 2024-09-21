using BepInEx;
using BepInEx.Bootstrap;
using HarmonyLib;

namespace CrewBoom
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInDependency(CharacterAPIGuid, BepInDependency.DependencyFlags.SoftDependency)]
    public class Plugin : BaseUnityPlugin
    {
        public const string CharacterAPIGuid = "com.Viliger.CharacterAPI";

        private void Awake()
        {
            if (Chainloader.PluginInfos.ContainsKey(CharacterAPIGuid))
            {
                Logger.LogWarning("CrewBoom is incompatible with CharacterAPI (viliger) and will not load!\nUninstall CharacterAPI and restart the game if you want to use CrewBoom.");
                return;
            }

            Logger.LogMessage($"{PluginInfo.PLUGIN_GUID} v{PluginInfo.PLUGIN_VERSION} starting...");

            CharacterDatabaseConfig.Initialize(Config);

            if (CharacterDatabase.Initialize())
            {
                Harmony harmony = new Harmony("softGoat.crewBoom");
                harmony.PatchAll();

                Logger.LogMessage($"Loaded all available characters!");
            }
        }
    }
}
