using BepInEx.Logging;
using CrewBoom.Data;
using HarmonyLib;
using Reptile;
using TMPro;

namespace CrewBoom.Patches
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
    [HarmonyPatch(typeof(TextMeshProGameTextLocalizer), nameof(TextMeshProGameTextLocalizer.GetSkinText))]
    public class SkinTextPatch
    {
        public static Characters Character;

        public const string SPRING_OUTFIT = "U_SKIN_SPRING";
        public const string SUMMER_OUTFIT = "U_SKIN_SUMMER";
        public const string AUTUMN_OUTFIT = "U_SKIN_AUTUMN";
        public const string WINTER_OUTFIT = "U_SKIN_WINTER";
        public static readonly string[] OUTFIT_NAMES = new string[]
        {
            SPRING_OUTFIT, SUMMER_OUTFIT, AUTUMN_OUTFIT, WINTER_OUTFIT
        };

        public static void Postfix(string localizationKey, ref string __result)
        {
            if (CharacterDatabase.GetCharacter(Character, out CustomCharacter characterObject))
            {
                int index;
                switch (localizationKey)
                {
                    case SPRING_OUTFIT:
                        index = 0;
                        break;
                    case SUMMER_OUTFIT:
                        index = 1;
                        break;
                    case AUTUMN_OUTFIT:
                        index = 2;
                        break;
                    case WINTER_OUTFIT:
                        index = 3;
                        break;
                    default:
                        return;
                }

                string outfitName = characterObject.Definition.Outfits[index].Name;
                if (outfitName != null && outfitName != string.Empty)
                {
                    __result = outfitName;
                }
            }
        }
    }
}
