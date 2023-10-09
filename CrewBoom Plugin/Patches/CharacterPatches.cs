using CrewBoom.Data;
using CrewBoomMono;
using HarmonyLib;
using Reptile;
using UnityEngine;

namespace CrewBoom.Patches
{
    [HarmonyPatch(typeof(Reptile.CharacterLoader), nameof(Reptile.CharacterLoader.GetCharacterFbx))]
    public class GetFbxPatch
    {
        public static void Postfix(Characters character, ref GameObject __result)
        {
            if (CharacterDatabase.GetCharacter(character, out CustomCharacter customCharacter))
            {
                __result = customCharacter.Definition.gameObject;
            }
        }
    }

    [HarmonyPatch(typeof(Reptile.CharacterVisual), nameof(Reptile.CharacterVisual.Init))]
    public class BlinkPatch
    {
        public static void Postfix(Characters character, ref bool ___canBlink)
        {
            if (CharacterDatabase.GetCharacter(character, out CustomCharacter customCharacter))
            {
                ___canBlink = customCharacter.Definition.CanBlink;
            }
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
            if (mode != CharacterVisual.MoveStylePropMode.ACTIVE)
            {
                return;
            }

            Player player = __instance.GetComponentInParent<Player>(true);
            if (player == null)
            {
                return;
            }

            Characters character = (Characters)player.GetField("character").GetValue(player);
            if (CharacterDatabase.HasCharacter(character))
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
