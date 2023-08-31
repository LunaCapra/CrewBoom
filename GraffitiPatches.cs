using BepInEx.Logging;
using HarmonyLib;
using Reptile;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace BrcCustomCharacters
{
    [HarmonyPatch(typeof(Reptile.GraffitiLoader), nameof(Reptile.GraffitiLoader.LoadGraffitiArtInfo))]
    public class GraffitiLoadPatch
    {
        public static void Postfix(GraffitiArtInfo ___graffitiArtInfo)
        {
            for (int i = 0; i < System.Enum.GetNames(typeof(Characters)).Length; i++)
            {
                Characters character = (Characters)i;
                if (CustomAssets.HasCharacter(character) && CustomAssets.HasGraffiti(character))
                {
                    GraffitiArt graffiti = ___graffitiArtInfo.FindByCharacter(character);
                    Material customGraffiti = CustomAssets.GetGraffiti(character);

                    Texture mainTex = customGraffiti.mainTexture;
                    graffiti.graffitiMaterial.mainTexture = mainTex;
                }
            }
        }
    }

    [HarmonyPatch(typeof(Reptile.GraffitiLoader), "LoadGraffitiArtInfoAsync")]
    public class GraffitiLoadAsyncPatch
    {
        private static IEnumerator Postfix(IEnumerator __result, Assets assets, GraffitiLoader __instance)
        {
            AssetBundleRequest graffitiArtInfoRequest = assets.LoadAssetFromBundleASync<GraffitiArtInfo>("graffiti", "graffitiartinfo");
            yield return graffitiArtInfoRequest;

            GraffitiArtInfo info = (GraffitiArtInfo)graffitiArtInfoRequest.asset;

            for (int i = 0; i < System.Enum.GetNames(typeof(Characters)).Length; i++)
            {
                Characters character = (Characters)i;
                if (CustomAssets.HasCharacter(character) && CustomAssets.HasGraffiti(character))
                {
                    GraffitiArt graffiti = info.FindByCharacter(character);
                    Material customGraffiti = CustomAssets.GetGraffiti(character);

                    Texture mainTex = customGraffiti.mainTexture;
                    graffiti.graffitiMaterial.mainTexture = mainTex;
                }
            }

            FieldInfo graffitiInfoField = LoadUtil.GetGraffitiInfo(__instance);
            graffitiInfoField.SetValue(__instance, graffitiArtInfoRequest.asset);

            yield break;
        }
    }

    //Note:
    //Just the UI text is patched for the new title rather than the actual title in the data
    //this is because when the title doesn't match what the game expects it can't load anything
    //It also breaks the graffiti if you play without the same character mod
    [HarmonyPatch(typeof(Reptile.GraffitiGame), "SetStateVisual")]
    public class GraffitiVisualPatch
    {
        public static void Postfix(GraffitiGame.GraffitiGameState setState, Player ___player, GraffitiArt ___grafArt, GraffitiArtInfo ___graffitiArtInfo)
        {
            if (setState == GraffitiGame.GraffitiGameState.SHOW_PIECE)
            {
                for (int i = 0; i < System.Enum.GetNames(typeof(Characters)).Length; i++)
                {
                    Characters character = (Characters)i;
                    if (CustomAssets.HasCharacter(character) && CustomAssets.HasGraffiti(character))
                    {
                        if (___grafArt == ___graffitiArtInfo.FindByCharacter(character))
                        {
                            Material customGraffiti = CustomAssets.GetGraffiti(character);
                            FieldInfo uiField = LoadUtil.GetField("ui", ___player);
                            GameplayUI ui = (GameplayUI)uiField.GetValue(___player);
                            ui.graffitiTitle.text = string.Format("'{0}'", customGraffiti.mainTexture.name);
                        }
                    }
                }
            }
        }
    }
}
