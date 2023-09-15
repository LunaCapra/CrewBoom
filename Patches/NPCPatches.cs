using UnityEngine;
using Reptile;
using HarmonyLib;
using BepInEx.Logging;
using BrcCustomCharactersLib;
using BrcCustomCharacters.Data;

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
                    if (CharacterDatabase.GetCharacter(npcCharacter.Character, out CustomCharacter character))
                    {
                        foreach (DynamicBone dynamicBone in npcCharacter.GetComponents<DynamicBone>())
                        {
                            dynamicBone.enabled = false;
                        }

                        GameObject customCharacter = Object.Instantiate(character.Definition.gameObject, npcCharacter.transform).gameObject;

                        Animator originalAnimator = npcCharacter.GetComponentInChildren<Animator>(true);
                        Animator customAnimator = customCharacter.GetComponent<Animator>();
                        customAnimator.runtimeAnimatorController = originalAnimator.runtimeAnimatorController;

                        customCharacter.transform.SetLocalPositionAndRotation(originalAnimator.transform.localPosition, originalAnimator.transform.localRotation);

                        SkinnedMeshRenderer customRenderer = customCharacter.GetComponentInChildren<SkinnedMeshRenderer>(true);
                        npcCharacter.SetField("mainRenderer", customRenderer);

                        customCharacter.AddComponent<LookAtIKComponent>();
                        customCharacter.AddComponent<DummyAnimationEventRelay>();
                        if (character.Definition.CanBlink)
                        {
                            StoryBlinkAnimation blinkAnimation = customCharacter.AddComponent<StoryBlinkAnimation>();
                            blinkAnimation.mainRenderer = customRenderer;
                            blinkAnimation.characterMesh = customRenderer.sharedMesh;
                        }

                        customCharacter.SetActive(originalAnimator.gameObject.activeSelf);

                        //Need to use DestroyImmediate because Destroy won't destroy it in time for the actual function running
                        //to not find the destroyed object still
                        Object.DestroyImmediate(originalAnimator.gameObject);
                    }
                }
            }
            else if (CharacterDatabase.GetCharacter(___character, out CustomCharacter character))
            {
                foreach (DynamicBone dynamicBone in __instance.GetComponentsInChildren<DynamicBone>(true))
                {
                    dynamicBone.enabled = false;
                }

                DummyAnimationEventRelay animatorBase = __instance.GetComponentInChildren<DummyAnimationEventRelay>(true);

                GameObject customCharacter = Object.Instantiate(character.Definition.gameObject, animatorBase.transform.parent).gameObject;

                Animator originalAnimator = animatorBase.GetComponent<Animator>();
                Animator customAnimator = customCharacter.GetComponent<Animator>();
                customAnimator.runtimeAnimatorController = originalAnimator.runtimeAnimatorController;

                MeshCollider collider = originalAnimator.GetComponent<MeshCollider>();

                customCharacter.transform.SetLocalPositionAndRotation(originalAnimator.transform.localPosition, originalAnimator.transform.localRotation);

                customCharacter.AddComponent<LookAtIKComponent>();
                customCharacter.AddComponent<DummyAnimationEventRelay>();
                if (collider)
                {
                    MeshCollider newCollider = customCharacter.AddComponent<MeshCollider>();
                    newCollider.sharedMesh = collider.sharedMesh;
                    newCollider.sharedMaterial = collider.sharedMaterial;
                    newCollider.convex = collider.convex;
                    newCollider.isTrigger = collider.isTrigger;
                }

                customCharacter.SetActive(originalAnimator.gameObject.activeSelf);

                //Need to use DestroyImmediate because Destroy won't destroy it in time for the actual function running
                //to not find the destroyed object still
                Object.DestroyImmediate(originalAnimator.gameObject);
            }
        }
    }
}
