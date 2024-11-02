using BepInEx.Logging;
using CrewBoom.Data;
using CrewBoomAPI;
using HarmonyLib;
using Reptile;
using System;
using UnityEngine;

namespace CrewBoom.Patches
{
    [HarmonyPatch(typeof(Reptile.Player), nameof(Reptile.Player.SetCharacter))]
    public class PlayerInitOverridePatch
    {
        public static void Prefix(ref Characters setChar)
        {
            if (CharacterDatabase.HasCharacterOverride)
            {
                if (CharacterDatabase.GetCharacterValueFromGuid(CharacterDatabase.CharacterOverride, out Characters character))
                {
                    if (character > Characters.MAX)
                    {
                        setChar = character;
                    }
                }
            }
        }

        public static void Postfix(Player __instance, Characters setChar)
        {
            if (CharacterDatabase.HasCharacterOverride)
            {
                CharacterDatabase.SetCharacterOverrideDone();
            }

            if (__instance == WorldHandler.instance.GetCurrentPlayer())
            {
                if (CharacterDatabase.GetCharacter(setChar, out CustomCharacter character))
                {
                    var info = new CrewBoomAPI.CharacterInfo(character.Definition.CharacterName, character.Definition.GraffitiName);
                    CrewBoomAPIDatabase.UpdatePlayerCharacter(info);
                }
                else
                {
                    CrewBoomAPIDatabase.UpdatePlayerCharacter(null);
                }
            }
        }
    }

    [HarmonyPatch(typeof(Reptile.Player), nameof(Reptile.Player.SetOutfit))]
    public class PlayerSetOutfitPatch
    {
        public static bool Prefix(int setOutfit, Player __instance, CharacterVisual ___characterVisual, Characters ___character)
        {
            if (!CharacterDatabase.HasCharacter(___character))
            {
                return true;
            }

            bool isAi = (bool) __instance.GetField("isAI").GetValue(__instance);
            if (!isAi)
            {
                Core.Instance.SaveManager.CurrentSaveSlot.GetCharacterProgress(___character).outfit = setOutfit;

                if (___character > Characters.MAX)
                {
                    if (CharacterDatabase.GetFirstOrConfigCharacterId(___character, out Guid guid))
                    {
                        CharacterSaveSlots.SaveCharacterData(guid);
                    }
                }
            }

            if (CharUtil.TrySetCustomOutfit(___characterVisual, setOutfit, out SkinnedMeshRenderer firstActiveRenderer))
            {
                ___characterVisual.mainRenderer = firstActiveRenderer;
            }

            return false;
        }
    }
    [HarmonyPatch(typeof(Reptile.Player), nameof(Reptile.Player.SetCurrentMoveStyleEquipped))]
    public class PlayerSetMovestyleEquipped
    {
        public static void Postfix(Player __instance, MoveStyle setMoveStyleEquipped)
        {
            bool isAi = (bool) __instance.GetField("isAI").GetValue(__instance);
            if (!isAi)
            {
                Characters character = (Characters) __instance.GetField("character").GetValue(__instance);
                if (character > Characters.MAX)
                {
                    if (CharacterDatabase.GetFirstOrConfigCharacterId(character, out Guid guid))
                    {
                        if (CharacterSaveSlots.GetCharacterData(guid, out CharacterProgress progress))
                        {
                            progress.moveStyle = setMoveStyleEquipped;
                            CharacterSaveSlots.SaveCharacterData(guid);
                        }
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(Reptile.Player), "SaveSelectedCharacter")]
    public class PlayerSaveCharacterPatch
    {
        public static bool Prefix(Player __instance, ref Characters selectedCharacter)
        {
            bool runOriginal = true;

            bool isAI = (bool) __instance.GetField("isAI").GetValue(__instance);
            bool isNew = selectedCharacter > Characters.MAX;
            if (!isAI)
            {
                CharacterSaveSlots.CurrentSaveSlot.LastPlayedCharacter = Guid.Empty;

                if (isNew)
                {
                    if (CharacterDatabase.GetFirstOrConfigCharacterId(selectedCharacter, out Guid guid))
                    {
                        CharacterSaveSlots.CurrentSaveSlot.LastPlayedCharacter = guid;
                    }
                    runOriginal = false;
                }

                CharacterSaveSlots.SaveSlot();
            }
            else if (selectedCharacter > Characters.MAX)
            {
                runOriginal = false;
            }

            return runOriginal;
        }
    }

    [HarmonyPatch(typeof(Reptile.Player), nameof(Player.PlayVoice))]
    public class PlayerVoicePatch
    {
        public static bool Prefix(AudioClipID audioClipID,
                                  VoicePriority voicePriority,
                                  bool fromPlayer,
                                  AudioManager ___audioManager,
                                  ref VoicePriority ___currentVoicePriority,
                                  Characters ___character,
                                  AudioSource ___playerGameplayVoicesAudioSource)
        {
            if (___character > Characters.MAX && CharacterDatabase.GetCharacter(___character, out CustomCharacter customCharacter))
            {
                if (fromPlayer)
                {
                    //ManualLogSource log = BepInEx.Logging.Logger.CreateLogSource("Test");
                    //log.LogMessage(___currentVoicePriority);

                    //___audioManager.InvokeMethod("PlayVoice",
                    //    new Type[] { typeof(VoicePriority).MakeByRefType(), typeof(Characters), typeof(AudioClipID), typeof(AudioSource), typeof(VoicePriority) },
                    //    ___currentVoicePriority, ___character, audioClipID, ___playerGameplayVoicesAudioSource, voicePriority);

                    //log.LogMessage(___currentVoicePriority);
                }
                else
                {
                    ___audioManager.InvokeMethod("PlaySfxGameplay",
                        new Type[] { typeof(SfxCollectionID), typeof(AudioClipID), typeof(float) },
                        customCharacter.SfxID, audioClipID, 0.0f);
                    return false;
                }

            }

            return true;
        }
    }
}
