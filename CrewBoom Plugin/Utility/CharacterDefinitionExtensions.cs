using BepInEx.Logging;
using CrewBoomMono;
using Reptile;
using UnityEngine;

namespace CrewBoom.Utility
{
    public static class CharacterDefinitionExtensions
    {
        public static bool HasVoices(this CharacterDefinition characterDefinition)
        {
            return characterDefinition.VoiceDie.Length > 0 ||
                   characterDefinition.VoiceDieFall.Length > 0 ||
                   characterDefinition.VoiceTalk.Length > 0 ||
                   characterDefinition.VoiceBoostTrick.Length > 0 ||
                   characterDefinition.VoiceCombo.Length > 0 ||
                   characterDefinition.VoiceGetHit.Length > 0 ||
                   characterDefinition.VoiceJump.Length > 0;
        }

        public static bool SetOutfit(this CharacterDefinition characterDefinition, int outfit, out SkinnedMeshRenderer firstActiveRenderer)
        {
            firstActiveRenderer = null;
            for (int i = 0; i < characterDefinition.Renderers.Length; i++)
            {
                SkinnedMeshRenderer renderer = characterDefinition.Renderers[i];
                renderer.sharedMaterials = characterDefinition.Outfits[outfit].MaterialContainers[i].Materials;
                renderer.gameObject.SetActive(characterDefinition.Outfits[outfit].EnabledRenderers[i]);

                if (firstActiveRenderer == null && characterDefinition.Outfits[outfit].EnabledRenderers[i])
                {
                    firstActiveRenderer = renderer;
                }
            }

            StoryBlinkAnimation blinkAnimation = characterDefinition.GetComponent<StoryBlinkAnimation>();
            if (blinkAnimation != null && firstActiveRenderer != null)
            {
                blinkAnimation.mainRenderer = firstActiveRenderer;
                blinkAnimation.characterMesh = firstActiveRenderer.sharedMesh;
            }

            return firstActiveRenderer != null;
        }
    }
}
