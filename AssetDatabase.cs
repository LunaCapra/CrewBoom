using UnityEngine;
using Reptile;
using System.IO;
using BrcCustomCharacters;
using System.Collections.Generic;
using System;
using BrcCustomCharactersLib;
using BepInEx.Logging;

public static class AssetDatabase
{
    private const string CHAR_ASSET_FOLDER = "brcCustomCharacters/CharAssets";
    private static string ASSET_PATH;

    private static Dictionary<Guid, string> _characterBundlePaths;
    private static Dictionary<Guid, string> _characterObjectNames;
    private static Dictionary<Guid, AssetBundle> _loadedBundles;
    private static Dictionary<Characters, List<Guid>> _characterReplacementIds;

    public static void Initialize(string pluginPath)
    {
        ASSET_PATH = Path.Combine(pluginPath, CHAR_ASSET_FOLDER);

        _characterBundlePaths = new Dictionary<Guid, string>();
        _characterObjectNames = new Dictionary<Guid, string>();
        _loadedBundles = new Dictionary<Guid, AssetBundle>();
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
        //InitializeAPI();
    }

    private static void LoadAllCharacterData()
    {
        ManualLogSource log = BepInEx.Logging.Logger.CreateLogSource("BRCCustomCharacters Loader");

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
                    if (objects.Length > 0)
                    {
                        Guid id = Guid.Parse(character.Id);
                        _characterBundlePaths.Add(id, filePath);
                        _characterObjectNames.Add(id, character.gameObject.name);
                        _characterReplacementIds[(Characters)character.CharacterToReplace].Add(id);

                        log.LogInfo($"Found character replacement \"{character.CharacterName}\" over {character.CharacterToReplace}");
                        log.LogInfo($"\tID: {id}");
                    }

                    bundle.Unload(true);
                }
            }
        }

        foreach (var bundle in AssetBundle.GetAllLoadedAssetBundles())
        {
            log.LogInfo($"Loaded bundle: {bundle.name}");
        }
    }

    //private static void InitializeAPI()
    //{
    //    Dictionary<int, Guid> userReplacements = new Dictionary<int, Guid>();

    //    var values = Enum.GetValues(typeof(Characters));
    //    for (int i = 0; i < values.Length; i++)
    //    {
    //        Characters character = (Characters)values.GetValue(i);
    //        if (GetFirstOrConfigCharacterId(character, out Guid id))
    //        {
    //            userReplacements.Add(i, id);
    //        }
    //    }

    //    BrcCustomCharactersAPI.Database.Initialize(userReplacements);
    //}

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

    private static AssetBundle GetCharacterBundle(Guid id)
    {
        if (!_loadedBundles.ContainsKey(id) || _loadedBundles[id] == null)
        {
            _loadedBundles[id] = AssetBundle.LoadFromFile(_characterBundlePaths[id]);
        }

        return _loadedBundles[id];
    }
    private static bool GetFirstOrConfigCharacterId(Characters character, out Guid guid)
    {
        guid = Guid.Empty;

        if (_characterReplacementIds[character] == null || _characterReplacementIds[character].Count == 0)
        {
            return false;
        }

        if (AssetConfig.GetCharacterOverride(character, out Guid id, out bool isDisabled))
        {
            if (_characterBundlePaths.ContainsKey(id) && _characterObjectNames.ContainsKey(id))
            {
                guid = id;
                return true;
            }
        }
        else
        {
            if (isDisabled)
            {
                return false;
            }
        }

        if (_characterReplacementIds[character].Count > 0)
        {
            guid = _characterReplacementIds[character][0];
            return true;
        }

        return false;
    }

    public static bool GetCharacterReplacement(Guid id, out CharacterDefinition characterObject)
    {
        characterObject = null;

        if (!_characterBundlePaths.ContainsKey(id))
        {
            return false;
        }

        AssetBundle characterBundle = GetCharacterBundle(id);
        if (characterBundle == null)
        {
            return false;
        }

        characterObject = characterBundle.LoadAsset<GameObject>(_characterBundlePaths[id]).GetComponent<CharacterDefinition>();
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
        List<Guid> characterIds = _characterReplacementIds[character];
        return characterIds != null && characterIds.Count > 0;
    }
}