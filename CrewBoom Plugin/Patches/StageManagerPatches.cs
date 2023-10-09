using BepInEx.Logging;
using HarmonyLib;
using Reptile;
using System;

namespace CrewBoom.Patches
{
    [HarmonyPatch(typeof(Reptile.StageManager), "SetupWorldHandler")]
    public class SetupPatch
    {
        public static void Postfix(WorldHandler ___worldHandler)
        {
            Player player = ___worldHandler.GetFieldValue<SceneObjectsRegister>("sceneObjectsRegister").players[0];
            Guid lastPlayedCharacter = CharacterSaveSlots.CurrentSaveSlot.LastPlayedCharacter;
            if (lastPlayedCharacter != Guid.Empty)
            {
                if (CharacterDatabase.GetCharacterValueFromGuid(lastPlayedCharacter, out Characters character))
                {
                    if (CharacterSaveSlots.GetCharacterData(lastPlayedCharacter, out CharacterProgress data))
                    {
                        ___worldHandler.InitPlayerObject(player, character, data.outfit, data.moveStyle);
                    }
                }
            }
        }
    }
}
