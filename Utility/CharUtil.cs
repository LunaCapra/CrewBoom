using Reptile;
using System.Collections.Generic;
using UnityEngine;

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

        public static string GetCharacterFromGraffitiTitle(Characters character)
        {
            switch (character)
            {
                case Characters.girl1:
                    return "Vinyl";
                case Characters.frank:
                case Characters.prince:
                    return "‘F’ Is For Family";
                case Characters.ringdude:
                    return "Coil";
                case Characters.metalHead:
                case Characters.legendMetalHead:
                    return "Red";
                case Characters.blockGuy:
                    return "Tryce";
                case Characters.spaceGirl:
                    return "Bel";
                case Characters.angel:
                    return "Rave";
                case Characters.eightBall:
                case Characters.eightBallBoss:
                    return "8O TUP";
                case Characters.dummy:
                    return "Solace";
                case Characters.dj:
                case Characters.futureGirl:
                    return "Eye-con";
                case Characters.medusa:
                    return "Elegant E";
                case Characters.boarder:
                    return "DT";
                case Characters.headMan:
                case Characters.headManNoJetpack:
                    return "Faux";
                case Characters.jetpackBossPlayer:
                    return "Irene";
                case Characters.legendFace:
                    return "Felix";
                case Characters.oldheadPlayer:
                    return "Boombap";
                case Characters.robot:
                    return "Base";
                case Characters.skate:
                    return "Jay";
                case Characters.wideKid:
                    return "Mesh";
                case Characters.pufferGirl:
                    return "Rise";
                case Characters.bunGirl:
                    return "Shine";
                default:
                    return "Red";
            }
        }

        private static readonly string[] PropBones = new string[]
        {
            "propl",
            "propr",
            "footl",
            "footr",
            "handl",
            "handr",
            "jetpackPos"
        };

        public static void ReparentAllProps(Transform originalRoot, Transform targetRoot)
        {
            foreach (string bone in PropBones)
            {
                Transform original = originalRoot.FindRecursive(bone);
                Transform target = targetRoot.FindRecursive(bone);
                ReparentChildren(original, target);
            }
        }
        private static void ReparentChildren(Transform source, Transform target)
        {
            Transform[] children = source.GetComponentsInChildren<Transform>();
            for (int i = 0; i < children.Length; i++)
            {
                children[i].SetParent(target, false);
                children[i].SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            }
        }
    }
}
