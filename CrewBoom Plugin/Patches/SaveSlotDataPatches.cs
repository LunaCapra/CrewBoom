using HarmonyLib;
using Reptile;
using System;

namespace CrewBoom.Patches
{
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
