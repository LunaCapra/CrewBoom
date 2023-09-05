using BepInEx.Logging;
using BrcCustomCharactersLib;
using HarmonyLib;
using Reptile;
using UnityEngine;

namespace BrcCustomCharacters.Patches
{
    [HarmonyPatch(typeof(Reptile.CharacterLoader), nameof(Reptile.CharacterLoader.GetCharacterFbx))]
    public class GetFbxPatch
    {
        public static void Postfix(Characters character, ref GameObject __result)
        {
            //if (BrcCustomCharactersAPI.Database.GetNextOverride(out System.Guid id))
            //{
            //    AssetDatabase.GetCharacterReplacement(id, out CharacterDefinition overrideCharacter);
            //    __result = overrideCharacter.gameObject;
            //    return;
            //}

            if (AssetDatabase.GetCharacterReplacement(character, out CharacterDefinition characterObject))
            {
                __result = characterObject.gameObject;
            }
        }
    }

    [HarmonyPatch(typeof(Reptile.CharacterLoader), nameof(Reptile.CharacterLoader.GetCharacterMaterial))]
    public class GetMaterialPatch
    {
        public static void Postfix(Characters character, int outfitIndex, ref Material __result)
        {
            //if (BrcCustomCharactersAPI.Database.GetNextOverride(out System.Guid id))
            //{
            //    AssetDatabase.GetCharacterReplacement(id, out CharacterDefinition overrideCharacter);
            //    __result = overrideCharacter.Outfits[outfitIndex];
            //    return;
            //}

            if (AssetDatabase.GetCharacterReplacement(character, out CharacterDefinition characterObject))
            {
                Material material = characterObject.Outfits[outfitIndex];
                if (characterObject.UseReptileShader)
                {
                    material.shader = __result.shader;
                }
                __result = material;
            }
        }
    }

    [HarmonyPatch(typeof(Reptile.CharacterVisual), nameof(Reptile.CharacterVisual.InitMoveStyleProps))]
    public class InlineSkatesTransformPatch
    {
        static void Postfix(
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
            if (AssetDatabase.HasCharacter(character))
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
