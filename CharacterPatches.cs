using BepInEx.Logging;
using HarmonyLib;
using Reptile;
using System.Collections;
using UnityEngine;

namespace BrcCustomCharacters
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
                fbxAsset = CustomAssets.GetCharacter(characterToLoad);
            }
            else
            {
                //Load default asset
                fbxAsset = assets.LoadAssetFromBundle<GameObject>(CharUtil.CHARACTER_BUNDLE, characterToLoad.ToString());
            }

            //Invoke private add fbx method
            LoadUtil.GetMethod(CharUtil.ADD_CHARACTER_METHOD, __instance).Invoke(__instance, new object[] { characterToLoad, fbxAsset });
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
                fbxAsset = CustomAssets.GetCharacter(characterToLoad);
            }
            else
            {
                //Load default asset
                AssetBundleRequest characterFbxAssetRequest = assets.LoadAssetFromBundleASync<GameObject>(CharUtil.CHARACTER_BUNDLE, characterToLoad.ToString());
                yield return characterFbxAssetRequest;

                fbxAsset = characterFbxAssetRequest.asset as GameObject;
            }

            //Invoke private add fbx method
            LoadUtil.GetMethod(CharUtil.ADD_CHARACTER_METHOD, __instance).Invoke(__instance, new object[] { characterToLoad, fbxAsset });

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
                Material customMaterial = CustomAssets.GetMaterial(characterToLoad, outfitIndex);

                //Set BRC shader
                customMaterial.shader = materialAsset.shader;

                materialAsset = customMaterial;
            }

            LoadUtil.GetMethod(CharUtil.ADD_MATERIAL_METHOD, __instance).Invoke(__instance, new object[] { characterToLoad, outfitIndex, materialAsset });
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
                Material customMaterial = CustomAssets.GetMaterial(characterToLoad, outfitIndex);

                //Set BRC shader
                customMaterial.shader = materialAsset.shader;

                materialAsset = customMaterial;
            }

            LoadUtil.GetMethod(CharUtil.ADD_MATERIAL_METHOD, __instance).Invoke(__instance, new object[] { characterToLoad, outfitIndex, materialAsset });

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
            string cloneName = __instance.transform.GetChild(0).name;
            bool found = System.Enum.TryParse<Characters>(cloneName.Remove(cloneName.Length - 7, 7), true, out Characters character);
            if (found && mode == CharacterVisual.MoveStylePropMode.ACTIVE && CustomAssets.HasCharacter(character))
            {
                Transform offsetL = ___footL.Find(CharUtil.SKATE_OFFSET_L);
                Transform offsetR = ___footR.Find(CharUtil.SKATE_OFFSET_R);

                if (offsetL != null && offsetR != null)
                {
                    ___moveStyleProps.skateL.transform.SetLocalPositionAndRotation(offsetL.localPosition, offsetL.localRotation);
                    ___moveStyleProps.skateR.transform.SetLocalPositionAndRotation(offsetR.localPosition, offsetR.localRotation);
                }
            }
        }
    }

    [HarmonyPatch(typeof(TextMeshProGameTextLocalizer), nameof(TextMeshProGameTextLocalizer.GetCharacterName), typeof(Characters))]
    public class CharacterNamePatch
    {
        public static void Postfix(Characters character, ref string __result)
        {
            if (CustomAssets.HasCharacter(character))
            {
                Transform nameObject = CustomAssets.GetCharacter(character).transform.Find("name");
                if (nameObject)
                {
                    Transform name = nameObject.GetChild(0);
                    __result = name.name;
                }
            }
        }
    }
}
