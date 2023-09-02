using UnityEngine;
using Reptile;
using HarmonyLib;

namespace BrcCustomCharacters
{
    [HarmonyPatch(typeof(Reptile.NPC), nameof(Reptile.NPC.InitSceneObject))]
    public class NPCInitPatch
    {
        public static void Prefix(NPC __instance, Characters ___character)
        {
            OutfitSwappableCharacter[] characters = __instance.GetComponentsInChildren<OutfitSwappableCharacter>(true);
            if (characters.Length > 0)
            {
                foreach (OutfitSwappableCharacter npc in characters)
                {
                    if (CustomAssets.HasCharacter(npc.Character))
                    {
                        foreach (DynamicBone dynamicBone in npc.GetComponents<DynamicBone>())
                        {
                            dynamicBone.enabled = false;
                        }

                        Animator animator = npc.GetComponentInChildren<Animator>(true);

                        GameObject customCharacter = Object.Instantiate(CustomAssets.GetCharacter(npc.Character), npc.transform);
                        customCharacter.transform.localPosition = animator.transform.localPosition;
                        customCharacter.transform.localRotation = animator.transform.localRotation;

                        Animator customAnimator = customCharacter.GetComponent<Animator>();
                        customAnimator.runtimeAnimatorController = animator.runtimeAnimatorController;
                        animator.avatar = customAnimator.avatar;

                        //Destroy original character stuff
                        foreach (Transform child in animator.transform)
                        {
                            Object.Destroy(child.gameObject);
                        }

                        SkinnedMeshRenderer characterRenderer = customCharacter.GetComponentInChildren<SkinnedMeshRenderer>(true);
                        npc.SetField("mainRenderer", characterRenderer);

                        StoryBlinkAnimation blinkAnimation = animator.gameObject.GetComponent<StoryBlinkAnimation>();
                        Object.Destroy(blinkAnimation);
                        //blinkAnimation.mainRenderer = characterRenderer;
                        //blinkAnimation.characterMesh = characterRenderer.sharedMesh;

                        foreach (Transform child in customCharacter.transform)
                        {
                            child.SetParent(animator.transform);
                        }

                        customCharacter.SetActive(animator.gameObject.activeSelf);
                    }
                }
            }
            else if (CustomAssets.HasCharacter(___character))
            {
                Animator animator = __instance.transform.GetComponentInChildren<Animator>(true);

                GameObject customCharacter = Object.Instantiate(CustomAssets.GetCharacter(___character), __instance.transform);
                customCharacter.transform.localPosition = animator.transform.localPosition;
                customCharacter.transform.localRotation = animator.transform.localRotation;

                Animator customAnimator = customCharacter.GetComponent<Animator>();
                customAnimator.runtimeAnimatorController = animator.runtimeAnimatorController;
                animator.avatar = customAnimator.avatar;

                SkinnedMeshRenderer originalRenderer = animator.GetComponentInChildren<SkinnedMeshRenderer>();
                if (originalRenderer)
                {
                    SkinnedMeshRenderer characterRenderer = customAnimator.GetComponentInChildren<SkinnedMeshRenderer>();
                    if (characterRenderer != null)
                    {
                        //Just copy the material, we already hijacked the material loading functions
                        characterRenderer.material = originalRenderer.material;
                    }
                }

                //Destroy original character stuff
                foreach (Transform child in animator.transform)
                {
                    if (child.GetComponentInChildren<Collider>() != null)
                    {
                        continue;
                    }
                    Object.Destroy(child.gameObject);
                }

                //Parent the new character to the animator
                foreach (Transform child in customCharacter.transform)
                {
                    child.SetParent(animator.transform);
                }

                customCharacter.SetActive(animator.gameObject.activeSelf);
            }
        }
    }
}
