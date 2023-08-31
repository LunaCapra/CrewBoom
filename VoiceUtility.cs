using Reptile;

namespace BrcCustomCharacters
{
    public static class VoiceUtility
    {
        public static SfxCollectionID VoiceCollectionFromCharacter(Characters character)
        {
            switch (character)
            {
                case Characters.girl1:
                    return SfxCollectionID.VoiceGirl1;
                case Characters.frank:
                    return SfxCollectionID.VoiceFrank;
                case Characters.ringdude:
                    return SfxCollectionID.VoiceRingDude;
                case Characters.metalHead:
                    return SfxCollectionID.VoiceMetalHead;
                case Characters.blockGuy:
                    return SfxCollectionID.VoiceBlockGuy;
                case Characters.spaceGirl:
                    return SfxCollectionID.VoiceSpaceGirl;
                case Characters.angel:
                    return SfxCollectionID.VoiceAngel;
                case Characters.eightBall:
                    return SfxCollectionID.VoiceEightBall;
                case Characters.dummy:
                    return SfxCollectionID.VoiceDummy;
                case Characters.dj:
                    return SfxCollectionID.VoiceDJ;
                case Characters.medusa:
                    return SfxCollectionID.VoiceMedusa;
                case Characters.boarder:
                    return SfxCollectionID.VoiceBoarder;
                case Characters.headMan:
                    return SfxCollectionID.VoiceHeadman;
                case Characters.prince:
                    return SfxCollectionID.VoicePrince;
                case Characters.jetpackBossPlayer:
                    return SfxCollectionID.VoiceJetpackBoss;
                case Characters.legendFace:
                    return SfxCollectionID.VoiceLegendFace;
                case Characters.oldheadPlayer:
                    return SfxCollectionID.NONE;
                case Characters.robot:
                    return SfxCollectionID.VoiceRobot;
                case Characters.skate:
                    return SfxCollectionID.VoiceSkate;
                case Characters.wideKid:
                    return SfxCollectionID.VoiceWideKid;
                case Characters.futureGirl:
                    return SfxCollectionID.VoiceFutureGirl;
                case Characters.pufferGirl:
                    return SfxCollectionID.VoicePufferGirl;
                case Characters.bunGirl:
                    return SfxCollectionID.VoiceBunGirl;
                case Characters.headManNoJetpack:
                    return SfxCollectionID.VoiceHeadmanNoJetpack;
                case Characters.eightBallBoss:
                    return SfxCollectionID.VoiceEightBallBoss;
                case Characters.legendMetalHead:
                    return SfxCollectionID.VoiceMetalHead;
            }

            return SfxCollectionID.NONE;
        }
    }
}