using BepInEx.Logging;
using CrewBoom.Data;
using HarmonyLib;
using Reptile;

namespace CrewBoom.Patches
{
    [HarmonyPatch(typeof(Reptile.CharacterVisual), nameof(CharacterVisual.GetCharacterFreestyleAnim))]
    public class CharacterFreestylePatch
    {
        public static void Prefix(ref Characters c)
        {
            if (CharacterDatabase.GetCharacter(c, out CustomCharacter customCharacter))
            {
                c = (Characters)customCharacter.Definition.FreestyleAnimation;
            }
        }
    }
    [HarmonyPatch(typeof(Reptile.CharacterVisual), nameof(CharacterVisual.GetCharacterBounceAnim))]
    public class CharacterBouncePatch
    {
        public static void Prefix(ref Characters c)
        {
            if (CharacterDatabase.GetCharacter(c, out CustomCharacter customCharacter))
            {
                c = (Characters)customCharacter.Definition.BounceAnimation;
            }
        }
    }
}
