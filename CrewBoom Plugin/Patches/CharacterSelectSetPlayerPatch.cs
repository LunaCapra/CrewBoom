using BepInEx.Logging;
using HarmonyLib;
using Reptile;
using System.Reflection;

namespace CrewBoom.Patches
{
    [HarmonyPatch(typeof(Reptile.CharacterSelect), "SetPlayerToCharacter")]
    public class CharacterSelectSetPlayerPatch
    {
        public static void Prefix(CharacterSelectCharacter[] ___charactersInCircle, int index, out Characters __state)
        {
            CharacterSelectCharacter characterSelectCharacter = ___charactersInCircle[index];
            __state = (Characters)characterSelectCharacter.GetField("character").GetValue(characterSelectCharacter);
        }

        public static void Postfix(Player ___player, Characters __state)
        {
            int outfit = Core.Instance.SaveManager.CurrentSaveSlot.GetCharacterProgress(__state).outfit;

            CharacterVisual visual = (CharacterVisual)___player.GetField("characterVisual").GetValue(___player);

            CharUtil.TrySetCustomOutfit(visual, outfit, out _);
        }
    }
}
