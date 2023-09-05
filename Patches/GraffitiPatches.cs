using BepInEx.Logging;
using BrcCustomCharactersLib;
using HarmonyLib;
using Reptile;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace BrcCustomCharacters.Patches
{
    [HarmonyPatch(typeof(Reptile.GraffitiLoader), nameof(Reptile.GraffitiLoader.LoadGraffitiArtInfo))]
    public class GraffitiLoadPatch
    {
        public static void Postfix(GraffitiArtInfo ___graffitiArtInfo)
        {
            for (int i = 0; i < System.Enum.GetValues(typeof(Characters)).Length - 1; i++)
            {
                Characters character = (Characters)i;
                if (AssetDatabase.GetCharacterReplacement(character, out CharacterDefinition characterObject))
                {
                    if (characterObject.Graffiti)
                    {
                        GraffitiArt graffiti = ___graffitiArtInfo.FindByCharacter(character);

                        Texture mainTex = characterObject.Graffiti.mainTexture;
                        graffiti.graffitiMaterial.mainTexture = mainTex;
                    }
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

            for (int i = 0; i < System.Enum.GetValues(typeof(Characters)).Length - 1; i++)
            {
                Characters character = (Characters)i;
                if (AssetDatabase.GetCharacterReplacement(character, out CharacterDefinition characterObject))
                {
                    if (characterObject.Graffiti)
                    {
                        GraffitiArt graffiti = info.FindByCharacter(character);

                        Texture mainTex = characterObject.Graffiti.mainTexture;
                        graffiti.graffitiMaterial.mainTexture = mainTex;
                    }
                }
            }

            __instance.SetField("graffitiArtInfo", graffitiArtInfoRequest.asset);

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
                for (int i = 0; i < System.Enum.GetValues(typeof(Characters)).Length - 1; i++)
                {
                    Characters character = (Characters)i;
                    if (AssetDatabase.GetCharacterReplacement(character, out CharacterDefinition characterObject))
                    {
                        if (characterObject.Graffiti)
                        {
                            if (___grafArt == ___graffitiArtInfo.FindByCharacter(character))
                            {
                                FieldInfo uiField = ___player.GetField("ui");
                                GameplayUI ui = (GameplayUI)uiField.GetValue(___player);
                                ui.graffitiTitle.text = string.Format("'{0}'", characterObject.Graffiti.mainTexture.name);
                            }
                        }
                    }
                }
            }
        }
    }
}