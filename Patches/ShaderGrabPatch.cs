using HarmonyLib;
using Reptile;
using System.Collections;
using UnityEngine;

namespace BrcCustomCharacters.Patches
{
    [HarmonyPatch(typeof(Reptile.CharacterLoader), "LoadMaterialForCharacter")]
    public class ShaderGrabSyncedPatch
    {
        public static void Postfix(Characters characterToLoad, int outfitIndex, Assets ___assets, CharacterLoader __instance)
        {
            Material materialAsset = ___assets.LoadAssetFromBundle<Material>("characters", CharUtil.GetOutfitMaterialName(characterToLoad, outfitIndex));

            if (!AssetDatabase.HasReptileShader())
            {
                AssetDatabase.SetReptileShader(materialAsset.shader);
            }

            __instance.InvokeMethod("AddCharacterMaterial", characterToLoad, outfitIndex, materialAsset);
        }
    }
    [HarmonyPatch(typeof(Reptile.CharacterLoader), "LoadMaterialsForCharacterASync")]
    public class ShaderGrabAsyncPatch
    {
        public static IEnumerator Postfix(IEnumerator __result, Characters characterToLoad, int outfitIndex, Assets ___assets, CharacterLoader __instance)
        {
            AssetBundleRequest characterMaterialRequest = ___assets.LoadAssetFromBundleASync<Material>("characters", CharUtil.GetOutfitMaterialName(characterToLoad, outfitIndex));
            yield return characterMaterialRequest;

            Material material = characterMaterialRequest.asset as Material;

            if (!AssetDatabase.HasReptileShader())
            {
                AssetDatabase.SetReptileShader(material.shader);
            }

            __instance.InvokeMethod("AddCharacterMaterial", characterToLoad, outfitIndex, material);

            yield break;
        }
    }
}
