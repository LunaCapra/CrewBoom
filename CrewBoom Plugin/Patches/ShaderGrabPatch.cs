using HarmonyLib;
using Reptile;
using System.Collections;
using UnityEngine;

namespace CrewBoom.Patches
{
    [HarmonyPatch(typeof(Reptile.CharacterLoader), "LoadMaterialForCharacter")]
    public class OutfitShaderGrab
    {
        public static void Postfix(Characters characterToLoad, int outfitIndex, Assets ___assets, CharacterLoader __instance)
        {
            Material materialAsset = ___assets.LoadAssetFromBundle<Material>("characters", CharUtil.GetOutfitMaterialName(characterToLoad, outfitIndex));

            CharacterDatabase.SetOutfitShader(materialAsset.shader);

            __instance.InvokeMethod("AddCharacterMaterial", characterToLoad, outfitIndex, materialAsset);
        }
    }
    [HarmonyPatch(typeof(Reptile.CharacterLoader), "LoadMaterialsForCharacterASync")]
    public class OutfitShaderGrabAsync
    {
        public static IEnumerator Postfix(IEnumerator __result, Characters characterToLoad, int outfitIndex, Assets ___assets, CharacterLoader __instance)
        {
            AssetBundleRequest characterMaterialRequest = ___assets.LoadAssetFromBundleASync<Material>("characters", CharUtil.GetOutfitMaterialName(characterToLoad, outfitIndex));
            yield return characterMaterialRequest;

            Material material = characterMaterialRequest.asset as Material;

            CharacterDatabase.SetOutfitShader(material.shader);

            __instance.InvokeMethod("AddCharacterMaterial", characterToLoad, outfitIndex, material);

            yield break;
        }
    }
}
