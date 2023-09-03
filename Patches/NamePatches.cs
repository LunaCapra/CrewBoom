using HarmonyLib;
using Reptile;
namespace BrcCustomCharacters.Patches
{
    [HarmonyPatch(typeof(TextMeshProGameTextLocalizer), nameof(TextMeshProGameTextLocalizer.GetCharacterName), typeof(Characters))]
    public class CharacterNamePatch
    {
        public static void Postfix(Characters character, ref string __result)
        {
            if (CustomAssets.GetCharacterName(character, out string name))
            {
                __result = name;
            }
        }
    }
    [HarmonyPatch(typeof(TextMeshProGameTextLocalizer), nameof(TextMeshProGameTextLocalizer.GetCharacterName), typeof(int))]
    public class CharacterKeyNamePatch
    {
        public static void Postfix(int localizationKeyID, ref string __result)
        {
            if (CustomAssets.GetCharacterNameWithId(localizationKeyID, out string name))
            {
                __result = name;
            }
        }
    }
}
