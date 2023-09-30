using BrcCustomCharactersLib;

namespace CrewBoom.Utility
{
    public static class CharacterDefinitionExtensions
    {
        public static bool HasVoices(this CharacterDefinition characterDefinition)
        {
            return characterDefinition.VoiceDie.Length > 0 ||
                   characterDefinition.VoiceDieFall.Length > 0 ||
                   characterDefinition.VoiceTalk.Length > 0 ||
                   characterDefinition.VoiceBoostTrick.Length > 0 ||
                   characterDefinition.VoiceCombo.Length > 0 ||
                   characterDefinition.VoiceGetHit.Length > 0 ||
                   characterDefinition.VoiceJump.Length > 0;
        }
    }
}
