using Reptile;
using HarmonyLib;
using System.Collections.Generic;
using System;
using CrewBoom.Data;
using UnityEngine;
using UnityEngine.Audio;
using BepInEx.Logging;

namespace CrewBoom.Patches
{
    [HarmonyPatch(typeof(Reptile.SfxLibrary), nameof(Reptile.SfxLibrary.Init))]
    public class InitSfxLibraryPatch
    {
        public static void Postfix(SfxLibrary __instance)
        {
            foreach (KeyValuePair<SfxCollectionID, SfxCollection> collectionPair in __instance.sfxCollectionIDDictionary)
            {
                Characters correspondingCharacter = VoiceUtility.CharacterFromVoiceCollection(collectionPair.Key);
                CharacterDatabase.InitializeMissingSfxCollections(correspondingCharacter, collectionPair.Value);
            }

            int max = (int)Characters.MAX;
            for (int i = max + 1; i <= max + CharacterDatabase.NewCharacterCount; i++)
            {
                Characters character = (Characters)i;
                if (CharacterDatabase.GetCharacter(character, out CustomCharacter customCharacter))
                {
                    if (customCharacter.SfxID != SfxCollectionID.NONE)
                    {
                        __instance.sfxCollectionIDDictionary.Add(customCharacter.SfxID, customCharacter.Sfx);
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(Reptile.SfxLibrary), nameof(Reptile.SfxLibrary.GetSfxCollectionById))]
    public class GetSfxCollectionIdPatch
    {
        public static void Postfix(SfxCollectionID sfxCollectionId, ref SfxCollection __result, SfxLibrary __instance)
        {
            Characters correspondingCharacter = VoiceUtility.CharacterFromVoiceCollection(sfxCollectionId);
            if (CharacterDatabase.GetCharacter(correspondingCharacter, out CustomCharacter customCharacter))
            {
                __result = customCharacter.Sfx;
            }
        }
    }

    [HarmonyPatch(typeof(Reptile.AudioManager), "GetCharacterVoiceSfxCollection")]
    public class GetSfxCharacterCollectionPatch
    {
        public static bool Prefix(Characters character, ref SfxCollectionID __result)
        {
            if (character > Characters.MAX)
            {
                if (CharacterDatabase.GetCharacter(character, out CustomCharacter customCharacter))
                {
                    __result = customCharacter.SfxID;
                    return false;
                }

                __result = SfxCollectionID.NONE;
                return false;
            }

            return true;
        }
    }
    [HarmonyPatch(typeof(Reptile.AudioManager), "PlayVoice")]
    [HarmonyPatch(new[] { typeof(VoicePriority), typeof(Characters), typeof(AudioClipID), typeof(AudioSource), typeof(VoicePriority) },
        new ArgumentType[] { ArgumentType.Ref, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal })]
    public class PlayVoicePatch
    {
        public static bool Prefix(ref VoicePriority currentPriority,
                                  Characters character,
                                  AudioClipID audioClipID,
                                  AudioSource audioSource,
                                  VoicePriority playbackPriority,
                                  AudioManager __instance,
                                  AudioMixerGroup[] ___mixerGroups)
        {
            if (character > Characters.MAX)
            {
                if (CharacterDatabase.GetCharacter(character, out CustomCharacter customCharacter))
                {
                    if (playbackPriority <= currentPriority && audioSource.isPlaying)
                    {
                        return false;
                    }

                    AudioClip clip = null;
                    if (customCharacter.Sfx != null)
                    {
                        clip = customCharacter.Sfx.GetRandomAudioClipById(audioClipID);
                    }

                    __instance.InvokeMethod("PlayNonloopingSfx",
                        new Type[] { typeof(AudioSource), typeof(AudioClip), typeof(AudioMixerGroup), typeof(float) },
                        audioSource, clip, ___mixerGroups[5], 0.0f);
                    currentPriority = playbackPriority;
                }
                return false;
            }

            return true;
        }
    }
    [HarmonyPatch(typeof(Reptile.AudioManager), "PlayVoice", typeof(Characters), typeof(AudioClipID))]
    public class PlayVoiceForCharacterPatch
    {
        public static bool Prefix(Characters character,
                                  AudioClipID audioClipID,
                                  AudioManager __instance,
                                  AudioMixerGroup[] ___mixerGroups,
                                  AudioSource[] ___audioSources,
                                  ref VoicePriority __result)
        {
            __result = VoicePriority.MOVEMENT;

            if (character > Characters.MAX)
            {
                if (CharacterDatabase.GetCharacter(character, out CustomCharacter customCharacter))
                {
                    AudioClip clip = null;
                    if (customCharacter.Sfx != null)
                    {
                        clip = customCharacter.Sfx.GetRandomAudioClipById(audioClipID);
                    }

                    __instance.InvokeMethod("PlayNonloopingSfx",
                        new Type[] { typeof(AudioSource), typeof(AudioClip), typeof(AudioMixerGroup), typeof(float) },
                        ___audioSources[5], clip, ___mixerGroups[5], 0.0f);
                }
                return false;
            }

            return true;
        }
    }
}
