using UnityEngine;
using Reptile;
using HarmonyLib;
using System.Collections.Generic;
using BrcCustomCharactersLib;

namespace BrcCustomCharacters.Patches
{
    [HarmonyPatch(typeof(Reptile.SfxLibrary), "GenerateEnumDictionary")]
    public class VoiceLoadPatch
    {
        public static void Postfix(SfxLibrary __instance)
        {
            foreach (KeyValuePair<SfxCollectionID, SfxCollection> collectionPair in __instance.sfxCollectionIDDictionary)
            {
                Characters correspondingCharacter = VoiceUtility.CharacterFromVoiceCollection(collectionPair.Key);
                if (AssetDatabase.GetCharacterReplacement(correspondingCharacter, out CharacterDefinition character))
                {
                    foreach (SfxCollection.RandomAudioClipContainer originalContainer in collectionPair.Value.audioClipContainers)
                    {
                        switch (originalContainer.clipID)
                        {
                            case AudioClipID.VoiceDie:
                                if (character.VoiceDie.Length > 0)
                                {
                                    originalContainer.clips = character.VoiceDie;
                                }
                                break;
                            case AudioClipID.VoiceDieFall:
                                if (character.VoiceDieFall.Length > 0)
                                {
                                    originalContainer.clips = character.VoiceDieFall;
                                }
                                break;
                            case AudioClipID.VoiceTalk:
                                if (character.VoiceTalk.Length > 0)
                                {
                                    originalContainer.clips = character.VoiceTalk;
                                }
                                break;
                            case AudioClipID.VoiceBoostTrick:
                                if (character.VoiceBoostTrick.Length > 0)
                                {
                                    originalContainer.clips = character.VoiceBoostTrick;
                                }
                                break;
                            case AudioClipID.VoiceCombo:
                                if (character.VoiceCombo.Length > 0)
                                {
                                    originalContainer.clips = character.VoiceCombo;
                                }
                                break;
                            case AudioClipID.VoiceGetHit:
                                if (character.VoiceGetHit.Length > 0)
                                {
                                    originalContainer.clips = character.VoiceGetHit;
                                }
                                break;
                            case AudioClipID.VoiceJump:
                                if (character.VoiceJump.Length > 0)
                                {
                                    originalContainer.clips = character.VoiceJump;
                                }
                                break;
                        }
                    }
                }
            }
        }
    }
}
