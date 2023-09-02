using UnityEngine;
using Reptile;
using HarmonyLib;
using System.Collections.Generic;

namespace BrcCustomCharacters
{
    [HarmonyPatch(typeof(Reptile.NPC), nameof(Reptile.NPC.InitSceneObject))]
    public class NPCInitPatch
    {
        public static void Postfix(NPC __instance, ref Transform ___head, Characters ___character)
        {
            OutfitSwappableCharacter[] characters = __instance.GetComponentsInChildren<OutfitSwappableCharacter>(true);
            if (characters.Length > 0)
            {
                foreach (OutfitSwappableCharacter npc in characters)
                {
                    if (CustomAssets.HasCharacter(npc.Character))
                    {
                        Animator animator = npc.GetComponentInChildren<Animator>(true);
                    }
                }
            }
        }
    }
}
