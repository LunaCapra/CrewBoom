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
                if (CustomAssets.HasCharacter(correspondingCharacter))
                {
                    foreach (SfxCollection.RandomAudioClipContainer originalContainer in collectionPair.Value.audioClipContainers)
                    {
                        CharacterDefinition characterDefinition = CustomAssets.GetCharacterReplacement(correspondingCharacter);

                        switch (originalContainer.clipID)
                        {
                            case AudioClipID.VoiceDie:
                                if (characterDefinition.VoiceDie.Length > 0)
                                {
                                    originalContainer.clips = characterDefinition.VoiceDie;
                                }
                                break;
                            case AudioClipID.VoiceDieFall:
                                if (characterDefinition.VoiceDieFall.Length > 0)
                                {
                                    originalContainer.clips = characterDefinition.VoiceDieFall;
                                }
                                break;
                            case AudioClipID.VoiceTalk:
                                if (characterDefinition.VoiceTalk.Length > 0)
                                {
                                    originalContainer.clips = characterDefinition.VoiceTalk;
                                }
                                break;
                            case AudioClipID.VoiceBoostTrick:
                                if (characterDefinition.VoiceBoostTrick.Length > 0)
                                {
                                    originalContainer.clips = characterDefinition.VoiceBoostTrick;
                                }
                                break;
                            case AudioClipID.VoiceCombo:
                                if (characterDefinition.VoiceCombo.Length > 0)
                                {
                                    originalContainer.clips = characterDefinition.VoiceCombo;
                                }
                                break;
                            case AudioClipID.VoiceGetHit:
                                if (characterDefinition.VoiceGetHit.Length > 0)
                                {
                                    originalContainer.clips = characterDefinition.VoiceGetHit;
                                }
                                break;
                            case AudioClipID.VoiceJump:
                                if (characterDefinition.VoiceJump.Length > 0)
                                {
                                    originalContainer.clips = characterDefinition.VoiceJump;
                                }
                                break;
                        }
                    }
                }
            }
        }
    }
}
