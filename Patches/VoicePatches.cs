using UnityEngine;
using Reptile;
using HarmonyLib;
using System.Collections.Generic;

namespace BrcCustomCharacters
{
    [HarmonyPatch(typeof(Reptile.SfxLibrary), "GenerateEnumDictionary")]
    public class VoiceLoadPatch
    {
        public static void Postfix(SfxLibrary __instance)
        {
            foreach (KeyValuePair<SfxCollectionID, SfxCollection> collectionPair in __instance.sfxCollectionIDDictionary)
            {
                if (CustomAssets.HasVoice(collectionPair.Key))
                {
                    foreach (SfxCollection.RandomAudioClipContainer originalContainer in collectionPair.Value.audioClipContainers)
                    {
                        CustomCharacterVoice voice = CustomAssets.GetVoice(collectionPair.Key);

                        switch (originalContainer.clipID)
                        {
                            case AudioClipID.VoiceDie:
                                if (voice.VoiceDie.Count > 0)
                                {
                                    originalContainer.clips = voice.VoiceDie.ToArray();
                                }
                                break;
                            case AudioClipID.VoiceDieFall:
                                if (voice.VoiceDieFall.Count > 0)
                                {
                                    originalContainer.clips = voice.VoiceDieFall.ToArray();
                                }
                                break;
                            case AudioClipID.VoiceTalk:
                                if (voice.VoiceTalk.Count > 0)
                                {
                                    originalContainer.clips = voice.VoiceTalk.ToArray();
                                }
                                break;
                            case AudioClipID.VoiceBoostTrick:
                                if (voice.VoiceBoostTrick.Count > 0)
                                {
                                    originalContainer.clips = voice.VoiceBoostTrick.ToArray();
                                }
                                break;
                            case AudioClipID.VoiceCombo:
                                if (voice.VoiceCombo.Count > 0)
                                {
                                    originalContainer.clips = voice.VoiceCombo.ToArray();
                                }
                                break;
                            case AudioClipID.VoiceGetHit:
                                if (voice.VoiceGetHit.Count > 0)
                                {
                                    originalContainer.clips = voice.VoiceGetHit.ToArray();
                                }
                                break;
                            case AudioClipID.VoiceJump:
                                if (voice.VoiceJump.Count > 0)
                                {
                                    originalContainer.clips = voice.VoiceJump.ToArray();
                                }
                                break;
                        }
                    }
                }
            }
        }
    }
}
