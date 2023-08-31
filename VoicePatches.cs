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
                                originalContainer.clips = voice.VoiceDie.ToArray();
                                break;
                            case AudioClipID.VoiceDieFall:
                                originalContainer.clips = voice.VoiceDieFall.ToArray();
                                break;
                            case AudioClipID.VoiceTalk:
                                originalContainer.clips = voice.VoiceTalk.ToArray();
                                break;
                            case AudioClipID.VoiceBoostTrick:
                                originalContainer.clips = voice.VoiceBoostTrick.ToArray();
                                break;
                            case AudioClipID.VoiceCombo:
                                originalContainer.clips = voice.VoiceCombo.ToArray();
                                break;
                            case AudioClipID.VoiceGetHit:
                                originalContainer.clips = voice.VoiceGetHit.ToArray();
                                break;
                            case AudioClipID.VoiceJump:
                                originalContainer.clips = voice.VoiceJump.ToArray();
                                break;
                        }
                    }
                }
            }
        }
    }
}
