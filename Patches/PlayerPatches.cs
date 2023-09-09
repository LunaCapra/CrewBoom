using BepInEx.Logging;
using BrcCustomCharactersLib;
using HarmonyLib;
using Reptile;

namespace BrcCustomCharacters.Patches
{
    [HarmonyPatch(typeof(Reptile.Player), nameof(Reptile.Player.Init))]
    public class PlayerInitOverridePatch
    {
        public static void Postfix()
        {
            if (AssetDatabase.HasCharacterOverride)
            {
                AssetDatabase.SetCharacterOverrideDone();
            }
        }
    }
}
