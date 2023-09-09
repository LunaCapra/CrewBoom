using BrcCustomCharactersLib;
using HarmonyLib;
using Reptile;
using System.Collections;
using System.Reflection;
using UnityEngine;

namespace BrcCustomCharacters.Patches
{
    [HarmonyPatch(typeof(Reptile.GraffitiLoader), nameof(Reptile.GraffitiLoader.LoadGraffitiArtInfo))]
    public class GraffitiLoadPatch
    {
        public static void Postfix(GraffitiLoader __instance)
        {
            Assets assets = (Assets)__instance.GetField("assets").GetValue(__instance);
            GraffitiArtInfo graffitiArtInfo = assets.LoadAssetFromBundle<GraffitiArtInfo>("graffiti", "graffitiartinfo");

            for (int i = 0; i < System.Enum.GetValues(typeof(Characters)).Length - 1; i++)
            {
                Characters character = (Characters)i;
                if (AssetDatabase.GetCharacterReplacement(character, out CharacterDefinition characterObject))
                {
                    if (characterObject.Graffiti)
                    {
                        GraffitiArt graffiti = graffitiArtInfo.FindByCharacter(character);

                        Texture mainTex = characterObject.Graffiti.mainTexture;
                        graffiti.title = characterObject.GraffitiName;
                        graffiti.graffitiMaterial.mainTexture = mainTex;
                    }
                }
            }

            __instance.SetField("graffitiArtInfo", graffitiArtInfo);
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
    //Patching just the UI title since the characters are replacements
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
                                ui.graffitiTitle.text = string.Format("'{0}'", characterObject.GraffitiName);
                            }
                        }
                    }
                }
            }
        }
    }
}