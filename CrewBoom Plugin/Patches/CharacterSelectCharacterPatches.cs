

using HarmonyLib;
using Reptile;
using UnityEngine;
using UnityEngine.TextCore.Text;

namespace CrewBoom.Patches
{
    [HarmonyPatch(typeof(Reptile.CharacterSelectCharacter), nameof(Reptile.CharacterSelectCharacter.Init))]
    public class CharacterSelectCharacterInitPatch
    {
        public static void Postfix(CharacterVisual ___visual, Characters setCharacter)
        {
            int outfit = Core.Instance.SaveManager.CurrentSaveSlot.GetCharacterProgress(setCharacter).outfit;
            CharUtil.TrySetCustomOutfit(___visual, outfit, out _);
        }
    }
}
