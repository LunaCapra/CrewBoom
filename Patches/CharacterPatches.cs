using BepInEx.Logging;
using HarmonyLib;
using Reptile;
using System.Collections;
using UnityEngine;

namespace BrcCustomCharacters.Patches
{
    [HarmonyPatch(typeof(Reptile.CharacterLoader), "LoadFBXForCharacter")]
    public class LoadingPatch
    {
        static void Postfix(Characters characterToLoad, CharacterLoader __instance)
        {
            Assets assets = LoadUtil.GetAssets(__instance);

            GameObject fbxAsset;
            if (CustomAssets.HasCharacter(characterToLoad))
            {
                //Load custom asset
                fbxAsset = CustomAssets.GetCharacterReplacement(characterToLoad).gameObject;
            }
            else
            {
                //Load default asset
                fbxAsset = assets.LoadAssetFromBundle<GameObject>(CharUtil.CHARACTER_BUNDLE, characterToLoad.ToString());
            }

            //Invoke private add fbx method
            __instance.InvokeMethod(CharUtil.ADD_CHARACTER_METHOD, new object[] { characterToLoad, fbxAsset });
        }
    }

    [HarmonyPatch(typeof(Reptile.CharacterLoader), "LoadFBXForCharacterASync")]
    public class AsyncLoadingPatch
    {
        static IEnumerator Postfix(IEnumerator __result, Characters characterToLoad, CharacterLoader __instance)
        {
            Assets assets = LoadUtil.GetAssets(__instance);

            GameObject fbxAsset;
            if (CustomAssets.HasCharacter(characterToLoad))
            {
                //Load custom asset
                fbxAsset = CustomAssets.GetCharacterReplacement(characterToLoad).gameObject;
            }
            else
            {
                //Load default asset
                AssetBundleRequest characterFbxAssetRequest = assets.LoadAssetFromBundleASync<GameObject>(CharUtil.CHARACTER_BUNDLE, characterToLoad.ToString());
                yield return characterFbxAssetRequest;

                fbxAsset = characterFbxAssetRequest.asset as GameObject;
            }

            //Invoke private add fbx method
            __instance.InvokeMethod(CharUtil.ADD_CHARACTER_METHOD, new object[] { characterToLoad, fbxAsset });

            yield break;
        }
    }

    [HarmonyPatch(typeof(Reptile.CharacterLoader), "LoadMaterialForCharacter")]
    public class MaterialPatch
    {
        static void Postfix(Characters characterToLoad, int outfitIndex, CharacterLoader __instance)
        {
            Assets assets = LoadUtil.GetAssets(__instance);

            Material materialAsset = assets.LoadAssetFromBundle<Material>(CharUtil.CHARACTER_BUNDLE, CharUtil.GetOutfitMaterialName(characterToLoad, outfitIndex));
            if (CustomAssets.HasCharacter(characterToLoad))
            {
                Material customMaterial = CustomAssets.GetCharacterReplacement(characterToLoad).Outfits[outfitIndex];

                //Set BRC shader
                customMaterial.shader = materialAsset.shader;

                materialAsset = customMaterial;
            }

            __instance.InvokeMethod(CharUtil.ADD_MATERIAL_METHOD, new object[] { characterToLoad, outfitIndex, materialAsset });
        }
    }

    [HarmonyPatch(typeof(Reptile.CharacterLoader), "LoadMaterialsForCharacterASync")]
    public class MaterialAsyncPatch
    {
        static IEnumerator Postfix(IEnumerator __result, Characters characterToLoad, int outfitIndex, CharacterLoader __instance)
        {
            Assets assets = LoadUtil.GetAssets(__instance);

            AssetBundleRequest characterMaterialRequest = assets.LoadAssetFromBundleASync<Material>(CharUtil.CHARACTER_BUNDLE, CharUtil.GetOutfitMaterialName(characterToLoad, outfitIndex));
            yield return characterMaterialRequest;

            Material materialAsset = characterMaterialRequest.asset as Material;
            if (CustomAssets.HasCharacter(characterToLoad))
            {
                Material customMaterial = CustomAssets.GetCharacterReplacement(characterToLoad).Outfits[outfitIndex];

                //Set BRC shader
                customMaterial.shader = materialAsset.shader;

                materialAsset = customMaterial;
            }

            __instance.InvokeMethod(CharUtil.ADD_MATERIAL_METHOD, new object[] { characterToLoad, outfitIndex, materialAsset });

            yield break;
        }
    }

    [HarmonyPatch(typeof(Reptile.CharacterVisual), "SetInlineSkatesPropsMode")]
    public class InlineSkatesTransformPatch
    {
        static void Postfix(CharacterVisual.MoveStylePropMode mode,
                            Transform ___footL,
                            Transform ___footR,
                            PlayerMoveStyleProps ___moveStyleProps,
                            CharacterVisual __instance)
        {
            Player player = __instance.GetComponentInParent<Player>(true);
            if (player == null)
            {
                return;
            }
            Characters character = (Characters)player.GetField("character").GetValue(player);
            if (mode == CharacterVisual.MoveStylePropMode.ACTIVE && CustomAssets.HasCharacter(character))
            {
                Transform offsetL = ___footL.Find(CharUtil.SKATE_OFFSET_L);
                Transform offsetR = ___footR.Find(CharUtil.SKATE_OFFSET_R);

                if (offsetL != null && offsetR != null)
                {
                    ___moveStyleProps.skateL.transform.SetLocalPositionAndRotation(offsetL.localPosition, offsetL.localRotation);
                    ___moveStyleProps.skateL.transform.localScale = offsetL.localScale;
                    ___moveStyleProps.skateR.transform.SetLocalPositionAndRotation(offsetR.localPosition, offsetR.localRotation);
                    ___moveStyleProps.skateR.transform.localScale = offsetR.localScale;
                }
            }
        }
    }
}
