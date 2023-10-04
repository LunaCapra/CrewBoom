

using CrewBoomMono;
using HarmonyLib;
using Reptile;
using UnityEngine;

namespace CrewBoom.Patches
{
    [HarmonyPatch(typeof(Reptile.OutfitSwitchMenu), nameof(Reptile.OutfitSwitchMenu.SkinButtonSelected))]
    public class OutfitSwitchMenuPatches
    {
        public static bool Prefix(OutfitSwitchMenu __instance, MenuTimelineButton ___buttonClicked, CharacterVisual ___previewCharacterVisual, int skinIndex)
        {
            if (!__instance.IsTransitioning && ___buttonClicked == null)
            {
                if (CharUtil.TrySetCustomOutfit(___previewCharacterVisual, skinIndex, out _))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
