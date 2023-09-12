using BrcCustomCharacters.Data;
using HarmonyLib;
using Reptile;
namespace BrcCustomCharacters.Patches
{
    [HarmonyPatch(typeof(TextMeshProGameTextLocalizer), nameof(TextMeshProGameTextLocalizer.GetCharacterName), typeof(Characters))]
    public class CharacterNamePatch
    {
        public static void Postfix(Characters character, ref string __result)
        {
            if (CharacterDatabase.GetCharacter(character, out CustomCharacter customCharacter))
            {
                __result = customCharacter.Definition.CharacterName;
            }
        }
    }
    [HarmonyPatch(typeof(TextMeshProGameTextLocalizer), nameof(TextMeshProGameTextLocalizer.GetCharacterName), typeof(int))]
    public class CharacterKeyNamePatch
    {
        public static void Postfix(int localizationKeyID, ref string __result)
        {
            if (CharacterDatabase.GetCharacterNameWithId(localizationKeyID, out string name))
            {
                __result = name;
            }
        }
    }
}
