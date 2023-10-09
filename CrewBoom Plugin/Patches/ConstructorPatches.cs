using BepInEx.Logging;
using CrewBoom.Data;
using HarmonyLib;
using Reptile;
using UnityEngine;

namespace CrewBoom.Patches
{
    [HarmonyPatch(typeof(Reptile.CharacterConstructor), nameof(Reptile.CharacterConstructor.CreateNewCharacterVisual))]
    public class ConstructorCreateVisualPatch
    {
        public static bool Prefix(Characters character,
                                   RuntimeAnimatorController controller,
                                   bool IK,
                                   float setGroundAngleLimit,
                                   ref CharacterVisual __result)
        {
            if (CharacterDatabase.GetCharacter(character, out CustomCharacter customCharacter))
            {
                CharacterVisual characterVisual = Object.Instantiate(customCharacter.Visual).AddComponent<CharacterVisual>();
                characterVisual.Init(character, controller, IK, setGroundAngleLimit);
                characterVisual.gameObject.SetActive(true);
                __result = characterVisual;

                return false;
            }

            return true;
        }
    }
}
