using Reptile;
using HarmonyLib;
using System.Collections.Generic;
using System;
using BrcCustomCharacters.Data;
using BepInEx.Logging;

namespace BrcCustomCharacters.Patches
{
    [HarmonyPatch(typeof(Reptile.SfxLibrary), nameof(Reptile.SfxLibrary.Init))]
    public class InitSfxLibraryPatch
    {
        public static void Postfix(SfxLibrary __instance)
        {
            ManualLogSource log = BepInEx.Logging.Logger.CreateLogSource("Voice test");

            foreach (KeyValuePair<SfxCollectionID, SfxCollection> collectionPair in __instance.sfxCollectionIDDictionary)
            {
                Characters correspondingCharacter = VoiceUtility.CharacterFromVoiceCollection(collectionPair.Key);
                AssetDatabase.InitializeMissingSfxCollections(correspondingCharacter, collectionPair.Value);
            }
        }
    }

    [HarmonyPatch(typeof(Reptile.SfxLibrary), nameof(Reptile.SfxLibrary.GetSfxCollectionById))]
    public class GetSfxCollectionIdPatch
    {
        public static void Postfix(SfxCollectionID sfxCollectionId, ref SfxCollection __result, SfxLibrary __instance)
        {
            Characters correspondingCharacter = VoiceUtility.CharacterFromVoiceCollection(sfxCollectionId);
            if (AssetDatabase.GetCharacter(correspondingCharacter, out CustomCharacter customCharacter))
            {
                __result = customCharacter.Sfx;
            }
        }
    }

    public class GetSfxCollectionStringPatch
    {
        public static void Postfix(string sfxCollectionName, ref SfxCollection __result, SfxLibrary __instance)
        {
            foreach (KeyValuePair<string, SfxCollection> stringPair in __instance.sfxCollectionDictionary)
            {
                if (!(stringPair.Value == null) && stringPair.Value.collectionName.Equals(sfxCollectionName))
                {
                    if (Enum.TryParse(stringPair.Key, out SfxCollectionID collectionId))
                    {
                        Characters correspondingCharacter = VoiceUtility.CharacterFromVoiceCollection(collectionId);
                        if (AssetDatabase.GetCharacter(correspondingCharacter, out CustomCharacter customCharacter))
                        {
                            __result = customCharacter.Sfx;
                        }
                    }
                }
            }
        }
    }
}
