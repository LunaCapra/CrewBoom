using UnityEngine;
using Reptile;
using HarmonyLib;
using BepInEx.Logging;
using BrcCustomCharactersLib;

namespace BrcCustomCharacters.Patches
{
    [HarmonyPatch(typeof(Reptile.NPC), nameof(Reptile.NPC.InitSceneObject))]
    public class NPCInitPatch
    {
        public static void Prefix(NPC __instance, Characters ___character)
        {
            OutfitSwappableCharacter[] characters = __instance.GetComponentsInChildren<OutfitSwappableCharacter>(true);
            if (characters.Length > 0)
            {
                foreach (OutfitSwappableCharacter npcCharacter in characters)
                {
                    if (AssetDatabase.GetCharacterReplacement(npcCharacter.Character, out CharacterDefinition character))
                    {
                        foreach (DynamicBone dynamicBone in npcCharacter.GetComponents<DynamicBone>())
                        {
                            dynamicBone.enabled = false;
                        }

                        GameObject customCharacter = Object.Instantiate(character, npcCharacter.transform).gameObject;

                        Animator originalAnimator = npcCharacter.GetComponentInChildren<Animator>(true);
                        Animator customAnimator = customCharacter.GetComponent<Animator>();
                        customAnimator.runtimeAnimatorController = originalAnimator.runtimeAnimatorController;

                        customCharacter.transform.localPosition = originalAnimator.transform.localPosition;
                        customCharacter.transform.localRotation = originalAnimator.transform.localRotation;

                        SkinnedMeshRenderer customRenderer = customCharacter.GetComponentInChildren<SkinnedMeshRenderer>(true);
                        npcCharacter.SetField("mainRenderer", customRenderer);

                        customCharacter.AddComponent<LookAtIKComponent>();
                        customCharacter.AddComponent<DummyAnimationEventRelay>();
                        //customCharacter.AddComponent<StoryBlinkAnimation>();

                        customCharacter.SetActive(originalAnimator.gameObject.activeSelf);

                        //Need to use DestroyImmediate because Destroy won't destroy it in time for the actual function running
                        //to not find the destroyed object still
                        Object.DestroyImmediate(originalAnimator.gameObject);
                    }
                }
            }
            else if (AssetDatabase.GetCharacterReplacement(___character, out CharacterDefinition character))
            {
                GameObject customCharacter = Object.Instantiate(character, __instance.transform).gameObject;

                Animator originalAnimator = __instance.transform.GetComponentInChildren<Animator>(true);
                Animator customAnimator = customCharacter.GetComponent<Animator>();
                customAnimator.runtimeAnimatorController = originalAnimator.runtimeAnimatorController;

                customCharacter.transform.localPosition = originalAnimator.transform.localPosition;
                customCharacter.transform.localRotation = originalAnimator.transform.localRotation;

                SkinnedMeshRenderer originalRenderer = originalAnimator.GetComponentInChildren<SkinnedMeshRenderer>();
                if (originalRenderer)
                {
                    SkinnedMeshRenderer characterRenderer = customAnimator.GetComponentInChildren<SkinnedMeshRenderer>();
                    if (characterRenderer != null)
                    {
                        //Just copy the material, we already hijacked the material loading functions
                        characterRenderer.material = originalRenderer.material;
                    }
                }

                customCharacter.SetActive(originalAnimator.gameObject.activeSelf);

                //Need to use DestroyImmediate because Destroy won't destroy it in time for the actual function running
                //to not find the destroyed object still
                Object.DestroyImmediate(originalAnimator.gameObject);
            }
        }
    }
}
