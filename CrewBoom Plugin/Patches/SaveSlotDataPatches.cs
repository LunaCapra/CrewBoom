using HarmonyLib;
using Reptile;
using System;
using System.IO;

namespace CrewBoom.Patches
{
    [HarmonyPatch(typeof(Reptile.SaveSlotData), nameof(Reptile.SaveSlotData.Write))]
    public class SaveSlotDataPatches
    {
        public static void Prefix(SaveSlotData __instance, ref Characters __state)
        {
            if (__instance.currentCharacter > Characters.MAX)
            {
                if (CharacterDatabase.GetFirstOrConfigCharacterId(__instance.currentCharacter, out Guid guid))
                {
                    CharacterSaveSlots.CurrentSaveSlot.LastPlayedCharacter = guid;
                    CharacterSaveSlots.SaveSlot();
                }

                //Save custom character for postfix
                __state = __instance.currentCharacter;

                //Make sure the game doesn't save a wrong character to its own save file
                __instance.currentCharacter = Characters.metalHead;
            }
        }

        public static void Postfix(SaveSlotData __instance, Characters __state)
        {
            if (__state > Characters.MAX)
            {
                //Restore custom character for in-game purposes
                __instance.currentCharacter = __state;
            }
        }
    }

    [HarmonyPatch(typeof(Reptile.SaveSlotData), nameof(Reptile.SaveSlotData.GetCharacterProgress))]
    public class SaveSlotGetProgressPatch
    {
        public static bool Prefix(Characters character, ref CharacterProgress __result)
        {
            if (character > Characters.MAX)
            {
                if (CharacterDatabase.GetFirstOrConfigCharacterId(character, out Guid guid))
                {
                    if (CharacterSaveSlots.GetCharacterData(guid, out CharacterProgress data))
                    {
                        __result = data;
                    }
                }
                return false;
            }

            return true;
        }
    }
}
