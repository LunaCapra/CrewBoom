using HarmonyLib;
using Reptile;
using UnityEngine;

namespace BrcCustomCharacters.Patches
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
            if (AssetDatabase.GetCharacterVisual(character, out GameObject characterVisualObject))
            {
                CharacterVisual characterVisual = Object.Instantiate(characterVisualObject).AddComponent<CharacterVisual>();
                characterVisual.Init(character, controller, IK, setGroundAngleLimit);
                characterVisual.gameObject.SetActive(true);
                __result = characterVisual;
                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(Reptile.CharacterConstructor), nameof(Reptile.CharacterConstructor.Dispose))]
    public class ConstructorDisposePatch
    {
        public static void Postfix()
        {
            AssetDatabase.DisposeOfVisuals();
        }
    }
}
