using UnityEngine;
using Reptile;
using System.IO;
using BrcCustomCharacters;
using System.Collections.Generic;
using System;
using BrcCustomCharactersLib;
using BepInEx.Logging;
using BrcCustomCharacters.Utility;

public static class AssetDatabase
{
    private const string CHAR_ASSET_FOLDER = "brcCustomCharacters/CharAssets";
    private static string ASSET_PATH;

    private static Dictionary<Guid, string> _characterBundlePaths;
    private static Dictionary<Guid, CharacterDefinition> _characterObjects;
    private static Dictionary<Guid, SfxCollection> _characterSfxCollections;
    private static Dictionary<Guid, GameObject> _characterVisuals;
    private static Dictionary<Characters, List<Guid>> _characterReplacementIds;

    private static Shader _reptileShader;

    public static bool HasCharacterOverride;
    private static Guid _currentCharacterOverride;

    private static ManualLogSource DebugLog = BepInEx.Logging.Logger.CreateLogSource("BRCCustomCharacters AssetDatabase");

    public static void Initialize(string pluginPath)
    {
        ASSET_PATH = Path.Combine(pluginPath, CHAR_ASSET_FOLDER);

        _characterBundlePaths = new Dictionary<Guid, string>();
        _characterObjects = new Dictionary<Guid, CharacterDefinition>();
        _characterVisuals = new Dictionary<Guid, GameObject>();
        _characterSfxCollections = new Dictionary<Guid, SfxCollection>();
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

        LoadAllCharacterData();
        InitializeAPI();
    }

    private static void LoadAllCharacterData()
    {
        if (!Directory.Exists(ASSET_PATH))
        {
            Directory.CreateDirectory(ASSET_PATH);
            return;
        }

        foreach (string filePath in Directory.GetFiles(ASSET_PATH))
        {
            if (File.Exists(filePath))
            {
                AssetBundle bundle = AssetBundle.LoadFromFile(filePath);
                if (bundle != null)
                {
                    GameObject[] objects = bundle.LoadAllAssets<GameObject>();
                    CharacterDefinition character = null;
                    foreach (GameObject obj in objects)
                    {
                        character = obj.GetComponent<CharacterDefinition>();
                        if (character != null)
                        {
                            break;
                        }
                    }
                    if (character != null)
                    {
                        DebugLog.LogInfo($"Found character replacement \"{character.CharacterName}\" over {character.CharacterToReplace}");
                        if (Guid.TryParse(character.Id, out Guid id))
                        {
                            _characterBundlePaths.Add(id, filePath);
                            //Store a reference to the path of this object so we don't have to go through all GameObjects again
                            _characterObjects.Add(id, character);
                            _characterReplacementIds[(Characters)character.CharacterToReplace].Add(id);

                            DebugLog.LogInfo($"\tID: {id}");
                        }
                        else
                        {
                            DebugLog.LogError($"\tThis character's GUID ({character.Id}) is invalid! Make sure their bundle was built correctly.");
                        }
                    }
                    else
                    {
                        DebugLog.LogWarning($"The asset bundle (\"{bundle.name}\") does not have a CharacterDefinition. You may be trying to load a character that was made with an older version of this plugin.");
                    }

                    //bundle.Unload(false);
                }
            }
        }
    }
    public static void InitializeSfxCollectionsForCharacter(Characters character, SfxCollection originalCollection)
    {
        if (!_characterReplacementIds.TryGetValue(character, out List<Guid> replacements))
        {
            return;
        }
        if (replacements == null || replacements.Count == 0)
        {
            return;
        }

        foreach (Guid guid in replacements)
        {
            if (GetCharacterReplacement(guid, out CharacterDefinition characterObject))
            {
                if (!characterObject.HasVoices())
                {
                    return;
                }

                SfxCollection newCollection = ScriptableObject.CreateInstance<SfxCollection>();
                newCollection.audioClipContainers = originalCollection.audioClipContainers;

                foreach (SfxCollection.RandomAudioClipContainer originalContainer in newCollection.audioClipContainers)
                {
                    switch (originalContainer.clipID)
                    {
                        case AudioClipID.VoiceDie:
                            if (characterObject.VoiceDie.Length > 0)
                            {
                                originalContainer.clips = characterObject.VoiceDie;
                            }
                            break;
                        case AudioClipID.VoiceDieFall:
                            if (characterObject.VoiceDieFall.Length > 0)
                            {
                                originalContainer.clips = characterObject.VoiceDieFall;
                            }
                            break;
                        case AudioClipID.VoiceTalk:
                            if (characterObject.VoiceTalk.Length > 0)
                            {
                                originalContainer.clips = characterObject.VoiceTalk;
                            }
                            break;
                        case AudioClipID.VoiceBoostTrick:
                            if (characterObject.VoiceBoostTrick.Length > 0)
                            {
                                originalContainer.clips = characterObject.VoiceBoostTrick;
                            }
                            break;
                        case AudioClipID.VoiceCombo:
                            if (characterObject.VoiceCombo.Length > 0)
                            {
                                originalContainer.clips = characterObject.VoiceCombo;
                            }
                            break;
                        case AudioClipID.VoiceGetHit:
                            if (characterObject.VoiceGetHit.Length > 0)
                            {
                                originalContainer.clips = characterObject.VoiceGetHit;
                            }
                            break;
                        case AudioClipID.VoiceJump:
                            if (characterObject.VoiceJump.Length > 0)
                            {
                                originalContainer.clips = characterObject.VoiceJump;
                            }
                            break;
                    }
                }

                _characterSfxCollections.Add(guid, newCollection);

                DebugLog.LogInfo($"Initialized voice library for \"{characterObject.CharacterName}\" (replaces {characterObject.CharacterToReplace})");
            }
        }
    }
    public static bool HasReptileShader()
    {
        return _reptileShader != null;
    }
    public static void SetReptileShader(Shader shader)
    {
        if (shader == null || _reptileShader != null)
        {
            return;
        }

        _reptileShader = shader;

        foreach (CharacterDefinition character in _characterObjects.Values)
        {
            if (character.UseReptileShader)
            {
                foreach (Material material in character.Outfits)
                {
                    material.shader = _reptileShader;
                }
            }
        }
    }
    private static GameObject ConstructCustomCharacterVisual(CharacterDefinition characterDefinition)
    {
        GameObject parent = new GameObject($"{characterDefinition.CharacterName} Visuals");
        GameObject characterModel = UnityEngine.Object.Instantiate(characterDefinition.gameObject);

        //InitCharacterModel
        characterModel.transform.SetParent(parent.transform, false);

        //InitSkinnedMeshRendererForModel
        SkinnedMeshRenderer meshRenderer = characterModel.GetComponentInChildren<SkinnedMeshRenderer>();
        meshRenderer.sharedMaterial = characterDefinition.Outfits[0];
        meshRenderer.receiveShadows = false;

        //InitAnimatorForModel
        characterModel.GetComponentInChildren<Animator>().applyRootMotion = false;

        //InitCharacterVisuals
        parent.SetActive(false);

        return parent;
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

        BrcCustomCharactersAPI.Database.Initialize(userReplacements);
        BrcCustomCharactersAPI.Database.OnOverride += SetCharacterOverride;
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

    public static bool GetCharacterName(Characters character, out string name)
    {
        name = string.Empty;

        if (GetCharacterReplacement(character, out CharacterDefinition characterObject))
        {
            name = characterObject.CharacterName;
            return true;
        }

        return false;
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

    private static bool GetFirstOrConfigCharacterId(Characters character, out Guid guid)
    {
        guid = Guid.Empty;

        if (HasCharacterOverride)
        {
            DebugLog.LogInfo($"Getting skin override for {character} with ID {_currentCharacterOverride}");
            if (_characterBundlePaths.ContainsKey(_currentCharacterOverride) && _characterObjects.ContainsKey(_currentCharacterOverride))
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
        if (AssetConfig.GetCharacterOverride(character, out Guid id, out bool isDisabled))
        {
            if (_characterBundlePaths.ContainsKey(id) && _characterObjects.ContainsKey(id))
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

    public static bool GetCharacterReplacement(Guid id, out CharacterDefinition characterObject)
    {
        if (!_characterObjects.TryGetValue(id, out characterObject))
        {
            return false;
        }

        return true;
    }
    public static bool GetCharacterReplacement(Characters character, out CharacterDefinition characterObject)
    {
        characterObject = null;

        if (GetFirstOrConfigCharacterId(character, out Guid guid))
        {
            GetCharacterReplacement(guid, out characterObject);
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
    public static bool GetCharacterVisual(Characters character, out GameObject characterVisualObject)
    {
        characterVisualObject = null;

        if (GetFirstOrConfigCharacterId(character, out Guid guid))
        {
            if (!_characterVisuals.TryGetValue(guid, out characterVisualObject))
            {
                if (GetCharacterReplacement(guid, out CharacterDefinition characterObject))
                {
                    _characterVisuals.Add(guid, ConstructCustomCharacterVisual(characterObject));
                    characterVisualObject = _characterVisuals[guid];
                }

                return true;
            }
            else
            {
                return true;
            }
        }

        return false;
    }
    public static void DisposeOfVisuals()
    {
        _characterVisuals.Clear();
    }
    public static bool GetCharacterSfxCollection(Characters character, out SfxCollection collection)
    {
        collection = null;

        if (GetFirstOrConfigCharacterId(character, out Guid id))
        {
            if (_characterSfxCollections.TryGetValue(id, out collection))
            {
                return true;
            }
        }

        return false;
    }
}