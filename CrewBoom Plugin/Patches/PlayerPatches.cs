using HarmonyLib;
using Reptile;
using UnityEngine;

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
    [HarmonyPatch(typeof(Reptile.Player), nameof(Reptile.Player.SetOutfit))]
    public class PlayerSetOutfitPatch
    {
        public static void Postfix(int setOutfit, CharacterVisual ___characterVisual)
        {
            if (CharUtil.TrySetCustomOutfit(___characterVisual, setOutfit, out SkinnedMeshRenderer firstActiveRenderer))
            {
                ___characterVisual.mainRenderer = firstActiveRenderer;
            }
        }
    }
}
