using UnityEngine;
using Reptile;
using HarmonyLib;
using CrewBoom.Data;
using CrewBoomMono;

namespace CrewBoom.Patches
{
    [HarmonyPatch(typeof(Reptile.NPC), nameof(Reptile.NPC.InitSceneObject))]
    public class NPCInitPatch
    {
        public static void Prefix(NPC __instance, Characters ___character)
        {
            OutfitSwappableCharacter[] characters = __instance.GetComponentsInChildren<OutfitSwappableCharacter>(true);
            if (characters != null && characters.Length > 0)
            {
                foreach (OutfitSwappableCharacter npcCharacter in characters)
                {
                    if (CharacterDatabase.GetCharacter(npcCharacter.Character, out CustomCharacter swappableCharacter))
                    {
                        foreach (DynamicBone dynamicBone in npcCharacter.GetComponents<DynamicBone>())
                        {
                            dynamicBone.enabled = false;
                        }

                        CharacterDefinition customCharacter = Object.Instantiate(swappableCharacter.Definition, npcCharacter.transform);

                        Animator originalAnimator = npcCharacter.GetComponentInChildren<Animator>(true);
                        Animator customAnimator = customCharacter.GetComponent<Animator>();
                        customAnimator.runtimeAnimatorController = originalAnimator.runtimeAnimatorController;

                        Transform root = originalAnimator.transform.Find("root");
                        if (root != null)
                        {
                            CharUtil.ReparentAllProps(root, customAnimator.transform.Find("root"));
                        }

                        customCharacter.transform.SetLocalPositionAndRotation(originalAnimator.transform.localPosition, originalAnimator.transform.localRotation);

                        SkinnedMeshRenderer customRenderer = customCharacter.Renderers[0];
                        npcCharacter.SetField("mainRenderer", customRenderer);

                        customCharacter.gameObject.AddComponent<LookAtIKComponent>();
                        customCharacter.gameObject.AddComponent<DummyAnimationEventRelay>();
                        if (swappableCharacter.Definition.CanBlink)
                        {
                            StoryBlinkAnimation blinkAnimation = customCharacter.gameObject.AddComponent<StoryBlinkAnimation>();
                            blinkAnimation.mainRenderer = customRenderer;
                            blinkAnimation.characterMesh = customRenderer.sharedMesh;
                        }
                        MeshCollider collider = originalAnimator.GetComponent<MeshCollider>();
                        if (collider)
                        {
                            MeshCollider newCollider = customCharacter.gameObject.AddComponent<MeshCollider>();
                            newCollider.sharedMesh = collider.sharedMesh;
                            newCollider.sharedMaterial = collider.sharedMaterial;
                            newCollider.convex = collider.convex;
                            newCollider.isTrigger = collider.isTrigger;
                        }

                        //Need to use DestroyImmediate because Destroy won't destroy it in time for the actual function running
                        //to not find the destroyed object still
                        Object.DestroyImmediate(originalAnimator.gameObject);
                    }
                }
            }
            else
            {
                ReplaceNonSwappableCharacter(__instance, ___character);
            }
        }

        private static void ReplaceNonSwappableCharacter(NPC __instance, Characters ___character)
        {
            if (CharacterDatabase.GetCharacter(___character, out CustomCharacter character))
            {
                DummyAnimationEventRelay animatorBase = __instance.GetComponentInChildren<DummyAnimationEventRelay>(true);
                if (animatorBase == null)
                {
                    return;
                }

                foreach (DynamicBone dynamicBone in __instance.GetComponentsInChildren<DynamicBone>(true))
                {
                    dynamicBone.enabled = false;
                }

                Animator originalAnimator = animatorBase.GetComponent<Animator>();

                GameObject customCharacter = Object.Instantiate(character.Definition.gameObject, originalAnimator.transform.parent).gameObject;

                Animator customAnimator = customCharacter.GetComponent<Animator>();
                customAnimator.runtimeAnimatorController = originalAnimator.runtimeAnimatorController;

                Transform root = originalAnimator.transform.Find("root");
                if (root != null)
                {
                    CharUtil.ReparentAllProps(root, customAnimator.transform.Find("root"));
                }

                customCharacter.transform.SetLocalPositionAndRotation(originalAnimator.transform.localPosition, originalAnimator.transform.localRotation);

                customCharacter.AddComponent<LookAtIKComponent>();
                customCharacter.AddComponent<DummyAnimationEventRelay>();
                MeshCollider collider = originalAnimator.GetComponent<MeshCollider>();
                if (collider)
                {
                    MeshCollider newCollider = customCharacter.AddComponent<MeshCollider>();
                    newCollider.sharedMesh = collider.sharedMesh;
                    newCollider.sharedMaterial = collider.sharedMaterial;
                    newCollider.convex = collider.convex;
                    newCollider.isTrigger = collider.isTrigger;
                }

                //Need to use DestroyImmediate because Destroy won't destroy it in time for the actual function running
                //to not find the destroyed object still
                Object.DestroyImmediate(originalAnimator.gameObject);
            }
        }
    }
}
