using UnityEngine;
using Reptile;
using System.IO;
using System.Collections.Generic;
using System;
using CrewBoomMonobehaviours;
using BepInEx.Logging;
using CrewBoom.Data;
using BepInEx;
using System.Text;
using CrewBoomAPI;

namespace CrewBoom
{
    public static class CharacterDatabase
    {
        private static string ASSET_PATH;

        private static Dictionary<Guid, string> _characterBundlePaths;
        private static Dictionary<Guid, CustomCharacter> _customCharacters;
        private static Dictionary<Characters, List<Guid>> _characterReplacementIds;

        public static bool HasCharacterOverride;
        private static Guid _currentCharacterOverride;

        private static ManualLogSource DebugLog = BepInEx.Logging.Logger.CreateLogSource($"{PluginInfo.PLUGIN_NAME} Database");

        public static bool Initialize()
        {
            ASSET_PATH = Path.Combine(Paths.ConfigPath, PluginInfo.PLUGIN_NAME);

            if (!Directory.Exists(ASSET_PATH))
            {
                DebugLog.LogWarning($"Could not find character bundle directory \"{ASSET_PATH}\".");
                Directory.CreateDirectory(ASSET_PATH);
                return false;
            }

            _characterBundlePaths = new Dictionary<Guid, string>();
            _customCharacters = new Dictionary<Guid, CustomCharacter>();
            _characterReplacementIds = new Dictionary<Characters, List<Guid>>();

            var charactersEnum = Enum.GetValues(typeof(Characters));
            foreach (Characters character in charactersEnum)
            {
                if (character == Characters.NONE || character == Characters.MAX)
                {
                    _characterReplacementIds.Add(character, null);
                    continue;
                }
                _characterReplacementIds.Add(character, new List<Guid>());
            }

            bool foundAnyCharacters = LoadAllCharacterData();
            if (!foundAnyCharacters)
            {
                return false;
            }

            InitializeAPI();

            return true;
        }

        private static bool LoadAllCharacterData()
        {
            bool foundAtLeastOneCharacter = false;

            foreach (string filePath in Directory.GetFiles(ASSET_PATH))
            {
                if (File.Exists(filePath))
                {
                    AssetBundle bundle = AssetBundle.LoadFromFile(filePath);
                    if (bundle != null)
                    {
                        GameObject[] objects = bundle.LoadAllAssets<GameObject>();
                        CharacterDefinition characterDefinition = null;
                        foreach (GameObject obj in objects)
                        {
                            characterDefinition = obj.GetComponent<CharacterDefinition>();
                            if (characterDefinition != null)
                            {
                                break;
                            }
                        }
                        if (characterDefinition != null)
                        {
                            StringBuilder characterLog = new StringBuilder();
                            characterLog.Append($"Loading \"{characterDefinition.CharacterName}\"");
                            characterLog.Append(characterDefinition.IsNewCharacter ? " as new character." : $" (Skin for {characterDefinition.CharacterToReplace})");
                            DebugLog.LogMessage(characterLog.ToString());

                            if (Guid.TryParse(characterDefinition.Id, out Guid id))
                            {
                                if (!foundAtLeastOneCharacter)
                                {
                                    foundAtLeastOneCharacter = true;
                                }

                                DebugLog.LogInfo($"\tGUID: {id}");

                                _characterBundlePaths.Add(id, filePath);
                                _characterReplacementIds[(Characters)characterDefinition.CharacterToReplace].Add(id);

                                //Create a new custom character instance and store it
                                CustomCharacter customCharacter = new CustomCharacter(characterDefinition);
                                _customCharacters.Add(id, customCharacter);
                            }
                            else
                            {
                                DebugLog.LogError($"\tThis character's GUID ({characterDefinition.Id}) is invalid! Make sure their bundle was built correctly.");
                            }
                        }
                        else
                        {
                            DebugLog.LogWarning($"The asset bundle at \"{filePath}\" does not have a CharacterDefinition. You may be trying to load a character that was made with an older version of this plugin.");
                        }

                        //bundle.Unload(false);
                    }
                }
            }

            return foundAtLeastOneCharacter;
        }

        public static void SetOutfitShader(Shader shader)
        {
            if (shader == null)
            {
                return;
            }

            foreach (CustomCharacter character in _customCharacters.Values)
            {
                if (character.Definition.UseReptileShader)
                {
                    character.ApplyShaderToOutfits(shader);
                }
            }
        }
        public static void SetGraffitiShader(Shader shader)
        {
            if (shader == null)
            {
                return;
            }

            foreach (CustomCharacter character in _customCharacters.Values)
            {
                character.ApplyShaderToGraffiti(shader);
            }
        }

        private static void InitializeAPI()
        {
            Dictionary<int, Guid> userReplacements = new Dictionary<int, Guid>();

            var values = Enum.GetValues(typeof(Characters));
            for (int i = 0; i < values.Length; i++)
            {
                Characters character = (Characters)values.GetValue(i);
                if (GetFirstOrConfigCharacterId(character, out Guid id))
                {
                    userReplacements.Add(i, id);
                }
            }

            CrewBoomAPIDatabase.Initialize(userReplacements);
            CrewBoomAPIDatabase.OnOverride += SetCharacterOverride;
        }
        private static void SetCharacterOverride(Guid id)
        {
            HasCharacterOverride = true;
            _currentCharacterOverride = id;

            DebugLog.LogInfo($"Received override for next character {id}");
        }
        public static void SetCharacterOverrideDone()
        {
            HasCharacterOverride = false;

            DebugLog.LogInfo("Finished override");
        }

        public static void InitializeMissingSfxCollections(Characters character, SfxCollection collection)
        {
            if (_characterReplacementIds.TryGetValue(character, out List<Guid> replacements))
            {
                if (replacements != null && replacements.Count > 0)
                {
                    foreach (Guid guid in replacements)
                    {
                        if (GetCharacter(guid, out CustomCharacter customCharacter))
                        {
                            customCharacter.ApplySfxCollection(collection);
                        }
                    }
                }
            }
        }

        public static bool GetCharacterNameWithId(int localizationId, out string name)
        {
            name = string.Empty;

            switch (localizationId)
            {
                case 0:
                    return false;
                case 6:
                    return GetCharacterName(Characters.girl1, out name);
                case 10:
                    return GetCharacterName(Characters.frank, out name);
                case 24:
                    return GetCharacterName(Characters.ringdude, out name);
                case 4:
                    return GetCharacterName(Characters.metalHead, out name);
                case 3:
                    return GetCharacterName(Characters.blockGuy, out name);
                case 5:
                    return GetCharacterName(Characters.spaceGirl, out name);
                case 26:
                    return GetCharacterName(Characters.angel, out name);
                case 12:
                    if (GetCharacterName(Characters.eightBall, out name))
                    {
                        return true;
                    }
                    else return GetCharacterName(Characters.eightBallBoss, out name);
                case 15:
                    return GetCharacterName(Characters.dummy, out name);
                case 7:
                    return GetCharacterName(Characters.dj, out name);
                case 11:
                    return GetCharacterName(Characters.medusa, out name);
                case 13:
                    return GetCharacterName(Characters.boarder, out name);
                case 2:
                    if (GetCharacterName(Characters.headMan, out name))
                    {
                        return true;
                    }
                    else return GetCharacterName(Characters.headManNoJetpack, out name);
                case 8:
                    return GetCharacterName(Characters.prince, out name);
                case 16:
                    return GetCharacterName(Characters.jetpackBossPlayer, out name);
                case 21:
                    return GetCharacterName(Characters.legendFace, out name);
                case 20:
                    return GetCharacterName(Characters.oldheadPlayer, out name);
                case 30:
                    return GetCharacterName(Characters.robot, out name);
                case 31:
                    return GetCharacterName(Characters.skate, out name);
                case 28:
                    return GetCharacterName(Characters.wideKid, out name);
                case 14:
                    return GetCharacterName(Characters.futureGirl, out name);
                case 25:
                    return GetCharacterName(Characters.pufferGirl, out name);
                case 27:
                    return GetCharacterName(Characters.bunGirl, out name);
                case 22:
                    return GetCharacterName(Characters.legendMetalHead, out name);
                default:
                    return false;
            }
        }
        private static bool GetCharacterName(Characters character, out string name)
        {
            name = string.Empty;

            if (GetCharacter(character, out CustomCharacter customCharacter))
            {
                name = customCharacter.Definition.CharacterName;
                return true;
            }

            return false;
        }

        private static bool GetFirstOrConfigCharacterId(Characters character, out Guid guid)
        {
            guid = Guid.Empty;

            if (HasCharacterOverride)
            {
                DebugLog.LogInfo($"Getting skin override for {character} with ID {_currentCharacterOverride}");
                if (_characterBundlePaths.ContainsKey(_currentCharacterOverride) && _customCharacters.ContainsKey(_currentCharacterOverride))
                {
                    DebugLog.LogInfo("Override was found locally.");
                    guid = _currentCharacterOverride;
                    return true;
                }
            }

            if (!_characterReplacementIds.TryGetValue(character, out List<Guid> replacements) ||
                replacements == null ||
                replacements.Count == 0)
            {
                return false;
            }

            //Check if the config has an override ID for this character
            if (CharacterConfig.GetCharacterOverride(character, out Guid id, out bool isDisabled))
            {
                if (_characterBundlePaths.ContainsKey(id) && _customCharacters.ContainsKey(id))
                {
                    guid = id;
                    return true;
                }
            }
            else
            {
                //If the override is OFF, ignore any skins for the local player
                if (isDisabled)
                {
                    return false;
                }
            }

            //If there's no override, just pick the first ID available
            guid = replacements[0];
            return true;
        }

        public static bool GetCharacter(Guid id, out CustomCharacter characterObject)
        {
            if (!_customCharacters.TryGetValue(id, out characterObject))
            {
                return false;
            }

            return true;
        }
        public static bool GetCharacter(Characters character, out CustomCharacter characterObject)
        {
            characterObject = null;

            if (GetFirstOrConfigCharacterId(character, out Guid guid))
            {
                GetCharacter(guid, out characterObject);
            }

            return characterObject != null;
        }
        public static bool HasCharacter(Characters character)
        {
            if (!_characterReplacementIds.TryGetValue(character, out List<Guid> replacements))
            {
                return false;
            }

            return replacements != null && replacements.Count > 0;
        }
    }
}