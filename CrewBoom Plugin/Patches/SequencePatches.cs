using BepInEx.Logging;
using CrewBoom.Data;
using CrewBoomMono;
using HarmonyLib;
using Reptile;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace CrewBoom.Patches
{
    [HarmonyPatch(typeof(Reptile.SequenceHandler), "ReplaceMaterialsOnCharactersInCutscene")]
    public class SequenceCharacterModelPatch
    {
        private static Dictionary<string, Characters> CutsceneOnlyCharacters = new Dictionary<string, Characters>
        {
            {"FauxNoJetpackStory", Characters.headManNoJetpack },
            {"FauxStory", Characters.headMan },
            {"SolaceStory", Characters.dummy },
            {"IreneStory", Characters.jetpackBossPlayer },
            {"DJMaskedStory", Characters.dj },
            {"DJNoMaskStory", Characters.dj },
            {"FuturismStory", Characters.futureGirl },
            {"FuturismBStory", Characters.futureGirl },
            {"FuturismCStory", Characters.futureGirl },
            {"FuturismDStory", Characters.futureGirl },
            {"EclipseAStory", Characters.medusa },
            {"EclipseBStory", Characters.medusa },
            {"EclipseCStory", Characters.medusa },
            {"EclipseDStory", Characters.medusa },
            {"DotExeEStory", Characters.eightBallBoss },
            {"DotExeAStory", Characters.eightBall },
            {"DotExeBStory", Characters.eightBall },
            {"DotExeCStory", Characters.eightBall },
            {"DotExeDStory", Characters.eightBall },
            {"RedShatteredStory", Characters.legendMetalHead },
            {"DemonTheoryAStory", Characters.boarder },
            {"DemonTheoryBStory", Characters.boarder },
            {"DemonTheoryCStory", Characters.boarder },
            {"FelixNoJetpackStory", Characters.legendFace },
            {"FrankAStory", Characters.frank },
            {"FrankBStory", Characters.frank },
            {"FrankCStory", Characters.frank },
            {"FrankDStory", Characters.frank },
            {"FleshPrinceStory", Characters.prince }
        };

        public static void Prefix(PlayableDirector ___sequence)
        {
            //Replace main characters
            foreach (OutfitSwappableCharacter character in ___sequence.GetComponentsInChildren<OutfitSwappableCharacter>(true))
            {
                SwapOutfitSwappable(character);
            }

            //Replace cutscene only characters
            Transform[] sceneObjects = ___sequence.GetComponentsInChildren<Transform>(true);
            foreach (Transform obj in sceneObjects)
            {
                foreach (string storyName in CutsceneOnlyCharacters.Keys)
                {
                    if (obj.name.StartsWith(storyName))
                    {
                        SwapCutsceneOnlyCharacter(obj, CutsceneOnlyCharacters[storyName]);
                    }
                }
            }
        }

        private static void SwapOutfitSwappable(OutfitSwappableCharacter swappable)
        {
            if (!CharacterDatabase.GetCharacter(swappable.Character, out CustomCharacter character))
            {
                return;
            }

            foreach (DynamicBone dynamicBone in swappable.transform.GetComponents<DynamicBone>())
            {
                dynamicBone.enabled = false;
            }

            GameObject customCharacter = Object.Instantiate(character.Definition.gameObject, swappable.transform).gameObject;

            Animator originalAnimator = null;
            foreach (Transform child in swappable.transform)
            {
                if ((originalAnimator = child.GetComponent<Animator>()) != null)
                {
                    break;
                }
            }
            Animator customAnimator = customCharacter.GetComponent<Animator>();

            originalAnimator.avatar = customAnimator.avatar;
            SkinnedMeshRenderer customRenderer = customAnimator.GetComponentInChildren<SkinnedMeshRenderer>(true);
            swappable.SetField("mainRenderer", customRenderer);

            foreach (Transform child in originalAnimator.transform)
            {
                //Weird unity hack witch magic ringle bingle ding pew pow workaround thing????
                //For some reason if you keep the old skeleton root, it doesn't force your character into a t-pose
                //Animator.Rebind() doesn't seem to work either because the object is disabled by default
                if (child.name == "root")
                {
                    child.name += "_old";

                    CharUtil.ReparentAllProps(child, customAnimator.transform.Find("root"));
                    continue;
                }
                Object.Destroy(child.gameObject);
            }

            customCharacter.transform.SetParent(originalAnimator.transform, false);
            customCharacter.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

            foreach (Transform child in customCharacter.GetComponentsInChildren<Transform>())
            {
                if (child.parent != customCharacter.transform)
                {
                    continue;
                }
                child.SetParent(originalAnimator.transform, false);
            }

            StoryBlinkAnimation blink = originalAnimator.GetComponent<StoryBlinkAnimation>();
            if (blink)
            {
                if (character.Definition.CanBlink)
                {
                    blink.mainRenderer = customRenderer;
                    blink.characterMesh = customRenderer.sharedMesh;
                }
                else
                {
                    blink.enabled = false;
                }
            }

            Object.Destroy(customCharacter);

            originalAnimator.Rebind(true);
        }
        private static void SwapCutsceneOnlyCharacter(Transform root, Characters character)
        {
            if (!CharacterDatabase.HasCharacter(character))
            {
                return;
            }

            OutfitSwappableCharacter swappable = root.gameObject.AddComponent<OutfitSwappableCharacter>();
            swappable.SetField("character", character);

            SwapOutfitSwappable(swappable);
        }
    }
}
