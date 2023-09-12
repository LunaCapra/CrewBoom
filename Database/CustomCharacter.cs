using BepInEx.Logging;
using BrcCustomCharacters.Utility;
using BrcCustomCharactersLib;
using Reptile;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BrcCustomCharacters.Data
{
    public class CustomCharacter
    {
        public CharacterDefinition Definition { get; private set; }
        public SfxCollection Sfx { get; private set; }
        public GameObject Visual
        {
            get
            {
                if (_visual == null)
                {
                    CreateVisual();
                }
                return _visual;
            }
        }
        private GameObject _visual;

        private static readonly List<AudioClipID> VOICE_IDS = new List<AudioClipID>()
        {
            AudioClipID.VoiceDie,
            AudioClipID.VoiceDieFall,
            AudioClipID.VoiceTalk,
            AudioClipID.VoiceBoostTrick,
            AudioClipID.VoiceCombo,
            AudioClipID.VoiceGetHit,
            AudioClipID.VoiceJump
        };

        public CustomCharacter(CharacterDefinition definition)
        {
            Definition = definition;

            CreateSfxCollection();

            ManualLogSource log = BepInEx.Logging.Logger.CreateLogSource("BrcCustomCharacters Character");
            if (Sfx != null)
            {
                log.LogInfo($"Created SfxCollection for \"{Definition.CharacterName}\"");
            }
        }

        private void CreateVisual()
        {
            GameObject parent = new GameObject($"{Definition.CharacterName} Visuals");
            GameObject characterModel = UnityEngine.Object.Instantiate(Definition.gameObject);

            //InitCharacterModel
            characterModel.transform.SetParent(parent.transform, false);

            //InitSkinnedMeshRendererForModel
            SkinnedMeshRenderer meshRenderer = characterModel.GetComponentInChildren<SkinnedMeshRenderer>();
            meshRenderer.sharedMaterial = Definition.Outfits[0];
            meshRenderer.receiveShadows = false;

            //InitAnimatorForModel
            characterModel.GetComponentInChildren<Animator>().applyRootMotion = false;

            //InitCharacterVisuals
            parent.SetActive(false);

            _visual = parent;
        }
        private void CreateSfxCollection()
        {
            if (!Definition.HasVoices())
            {
                return;
            }

            SfxCollection newCollection = ScriptableObject.CreateInstance<SfxCollection>();
            newCollection.audioClipContainers = new SfxCollection.RandomAudioClipContainer[VOICE_IDS.Count];
            for (int i = 0; i < VOICE_IDS.Count; i++)
            {
                newCollection.audioClipContainers[i] = new SfxCollection.RandomAudioClipContainer();
                newCollection.audioClipContainers[i].clipID = VOICE_IDS[i];
                newCollection.audioClipContainers[i].clips = null;
                newCollection.audioClipContainers[i].lastRandomClip = 0;
            }

            foreach (SfxCollection.RandomAudioClipContainer originalContainer in newCollection.audioClipContainers)
            {
                switch (originalContainer.clipID)
                {
                    case AudioClipID.VoiceDie:
                        if (Definition.VoiceDie.Length > 0)
                        {
                            originalContainer.clips = Definition.VoiceDie;
                        }
                        break;
                    case AudioClipID.VoiceDieFall:
                        if (Definition.VoiceDieFall.Length > 0)
                        {
                            originalContainer.clips = Definition.VoiceDieFall;
                        }
                        break;
                    case AudioClipID.VoiceTalk:
                        if (Definition.VoiceTalk.Length > 0)
                        {
                            originalContainer.clips = Definition.VoiceTalk;
                        }
                        break;
                    case AudioClipID.VoiceBoostTrick:
                        if (Definition.VoiceBoostTrick.Length > 0)
                        {
                            originalContainer.clips = Definition.VoiceBoostTrick;
                        }
                        break;
                    case AudioClipID.VoiceCombo:
                        if (Definition.VoiceCombo.Length > 0)
                        {
                            originalContainer.clips = Definition.VoiceCombo;
                        }
                        break;
                    case AudioClipID.VoiceGetHit:
                        if (Definition.VoiceGetHit.Length > 0)
                        {
                            originalContainer.clips = Definition.VoiceGetHit;
                        }
                        break;
                    case AudioClipID.VoiceJump:
                        if (Definition.VoiceJump.Length > 0)
                        {
                            originalContainer.clips = Definition.VoiceJump;
                        }
                        break;
                }
            }

            Sfx = newCollection;
        }

        public void ApplySfxCollection(SfxCollection collection)
        {
            if (Sfx == null)
            {
                Sfx = collection;
                return;
            }
            else
            {
                foreach (SfxCollection.RandomAudioClipContainer container in collection.audioClipContainers)
                {
                    if (!VOICE_IDS.Contains(container.clipID))
                    {
                        Array.Resize(ref Sfx.audioClipContainers, Sfx.audioClipContainers.Length + 1);
                        Sfx.audioClipContainers[Sfx.audioClipContainers.Length] = container;
                    }
                    //else
                    //{
                    //    SfxCollection.RandomAudioClipContainer correspondingContainer = Sfx.audioClipContainers.First(x => x.clipID == container.clipID);
                    //    if (correspondingContainer.clips == null)
                    //    {
                    //        correspondingContainer.clips = container.clips;
                    //    }
                    //}
                }
            }
        }
        public void ApplyShaderToOutfits(Shader shader)
        {
            foreach (Material material in Definition.Outfits)
            {
                material.shader = shader;
            }
        }
    }
}
