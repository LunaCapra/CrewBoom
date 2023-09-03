using Reptile;
using System.Collections.Generic;

namespace BrcCustomCharacters
{
    public static class CharUtil
    {
        public const string CHARACTER_BUNDLE = "characters";

        public const string ADD_CHARACTER_METHOD = "AddCharacterFBX";
        public const string ADD_MATERIAL_METHOD = "AddCharacterMaterial";

        public const string MATERIAL_FORMAT = "{0}Mat{1}";

        public const string SKATE_OFFSET_L = "skateOffsetL";
        public const string SKATE_OFFSET_R = "skateOffsetR";

        public const string GRAFFITI_ASSET = "charGraffiti";

        public static string GetOutfitMaterialName(Characters character, int outfitIndex)
        {
            return string.Format(MATERIAL_FORMAT, character.ToString(), outfitIndex.ToString());
        }
    }
}
