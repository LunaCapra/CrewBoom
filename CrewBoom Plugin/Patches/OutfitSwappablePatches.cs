using HarmonyLib;
using Reptile;
using UnityEngine;

namespace CrewBoom.Patches
{
    [HarmonyPatch(typeof(Reptile.OutfitSwappableCharacter), nameof(Reptile.OutfitSwappableCharacter.SetMaterialAndOutfit))]
    public class OutfitSwappablePatches
    {
        public static bool Prefix(OutfitSwappableCharacter __instance, ref SkinnedMeshRenderer ___mainRenderer, int outfitIndex)
        {
            if (CharUtil.TrySetCustomOutfit(__instance, outfitIndex, out SkinnedMeshRenderer firstActiveRenderer))
            {
                ___mainRenderer = firstActiveRenderer;
                return false;
            }

            return true;
        }
    }
}
