using HarmonyLib;

namespace CrewBoom.Patches
{
    [HarmonyPatch(typeof(Reptile.Player), nameof(Reptile.Player.Init))]
    public class PlayerInitOverridePatch
    {
        public static void Postfix()
        {
            if (CharacterDatabase.HasCharacterOverride)
            {
                CharacterDatabase.SetCharacterOverrideDone();
            }
        }
    }
}
