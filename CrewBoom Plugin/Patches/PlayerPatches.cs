using BepInEx.Logging;
using CrewBoom.Data;
using HarmonyLib;
using Reptile;
using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.TextCore.Text;

namespace CrewBoom.Patches
{
    [HarmonyPatch(typeof(Reptile.Player), nameof(Reptile.Player.Init))]
    public class PlayerInitOverridePatch
    {
        public static void Postfix()
        {
            if (CharacterDatabase.HasCharacterOverride)
            {
                CharacterDatabase.SetCharacterOverrideDone();
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

            bool isAi = (bool)__instance.GetField("isAI").GetValue(__instance);
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
            bool isAi = (bool)__instance.GetField("isAI").GetValue(__instance);
            if (!isAi)
            {
                Characters character = (Characters)__instance.GetField("character").GetValue(__instance);
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
        public static bool Prefix(ref Characters selectedCharacter)
        {
            if (selectedCharacter > Characters.MAX)
            {
                if (CharacterDatabase.GetFirstOrConfigCharacterId(selectedCharacter, out Guid guid))
                {
                    CharacterSaveSlots.CurrentSaveSlot.LastPlayedCharacter = guid;
                    CharacterSaveSlots.SaveSlot();
                }

                return false;
            }

            return true;
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
