using UnityEngine;
using Reptile;
using System.IO;
using BrcCustomCharacters;
using System.Collections.Generic;

public static class CustomAssets
{
    private const string CHAR_ASSET_FOLDER = "brcCustomCharacters/CharAssets";
    private static string ASSET_PATH;

    private const string VOICE_DIE_SUFFIX = "_die";
    private const string VOICE_DIEFALL_SUFFIX = "_dieFall";
    private const string VOICE_TALK_SUFFIX = "_talk";
    private const string VOICE_BOOSTTRICK_SUFFIX = "_bTrick";
    private const string VOICE_COMBO_SUFFIX = "_combo";
    private const string VOICE_GETHIT_SUFFIX = "_hit";
    private const string VOICE_JUMP_SUFFIX = "_jump";

    private static AssetBundle[] _characterBundles;
    private static Material[] _characterGraffiti;
    private static Dictionary<SfxCollectionID, CustomCharacterVoice> _characterVoices;

    public static void Initialize(string pluginPath)
    {
        ASSET_PATH = Path.Combine(pluginPath, CHAR_ASSET_FOLDER);

        string[] characterEnum = System.Enum.GetNames(typeof(Characters));
        _characterBundles = new AssetBundle[characterEnum.Length];
        _characterGraffiti = new Material[characterEnum.Length];
        _characterVoices = new Dictionary<SfxCollectionID, CustomCharacterVoice>();

        for (int i = 0; i < characterEnum.Length; i++)
        {
            if (LoadCharacterBundle((Characters)i, out AssetBundle characterBundle))
            {
                _characterBundles[i] = characterBundle;

                Material graffiti = _characterBundles[i].LoadAsset<Material>(CharUtil.GRAFFITI_ASSET);
                _characterGraffiti[i] = graffiti;

                AudioClip[] allVoices = _characterBundles[i].LoadAllAssets<AudioClip>();
                if (allVoices.Length > 0)
                {
                    SfxCollectionID key = VoiceUtility.VoiceCollectionFromCharacter((Characters)i);
                    _characterVoices.Add(key, new CustomCharacterVoice());
                    foreach (AudioClip voice in allVoices)
                    {
                        if (voice.name.EndsWith(VOICE_DIE_SUFFIX))
                        {
                            _characterVoices[key].VoiceDie.Add(voice);
                        }
                        if (voice.name.EndsWith(VOICE_DIEFALL_SUFFIX))
                        {
                            _characterVoices[key].VoiceDieFall.Add(voice);
                        }
                        if (voice.name.EndsWith(VOICE_TALK_SUFFIX))
                        {
                            _characterVoices[key].VoiceTalk.Add(voice);
                        }
                        if (voice.name.EndsWith(VOICE_BOOSTTRICK_SUFFIX))
                        {
                            _characterVoices[key].VoiceBoostTrick.Add(voice);
                        }
                        if (voice.name.EndsWith(VOICE_COMBO_SUFFIX))
                        {
                            _characterVoices[key].VoiceCombo.Add(voice);
                        }
                        if (voice.name.EndsWith(VOICE_GETHIT_SUFFIX))
                        {
                            _characterVoices[key].VoiceGetHit.Add(voice);
                        }
                        if (voice.name.EndsWith(VOICE_JUMP_SUFFIX))
                        {
                            _characterVoices[key].VoiceJump.Add(voice);
                        }
                    }
                }
            }
        }
    }

    private static bool LoadCharacterBundle(Characters character, out AssetBundle bundle)
    {
        bundle = null;

        string filePath = Path.Combine(ASSET_PATH, character.ToString().ToLower());

        if (File.Exists(filePath))
        {
            AssetBundle characterBundle = AssetBundle.LoadFromFile(filePath);
            bundle = characterBundle;
            return true;
        }

        return false;
    }

    public static bool HasCharacter(Characters character)
    {
        //Catch Characters.NONE as it is -1
        if (character == Characters.NONE) return false;
        return _characterBundles[(int)character] != null;
    }

    public static bool GetCharacterName(Characters character, out string name)
    {
        name = string.Empty;

        if (HasCharacter(character))
        {
            Transform nameObject = GetCharacter(character).transform.Find("name");
            if (nameObject)
            {
                Transform text = nameObject.GetChild(0);
                name = text.name;

                return true;
            }
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

    public static bool HasGraffiti(Characters character)
    {
        return _characterGraffiti[(int)character] != null;
    }

    public static bool HasVoice(SfxCollectionID id)
    {
        return _characterVoices.ContainsKey(id);
    }

    public static GameObject GetCharacter(Characters character)
    {
        return _characterBundles[(int)character].LoadAsset<GameObject>(character.ToString());
    }

    public static Material GetMaterial(Characters character, int outfitIndex)
    {
        return _characterBundles[(int)character].LoadAsset<Material>(CharUtil.GetOutfitMaterialName(character, outfitIndex));
    }

    public static Material GetGraffiti(Characters character)
    {
        return _characterGraffiti[(int)character];
    }

    public static CustomCharacterVoice GetVoice(SfxCollectionID id)
    {
        _characterVoices.TryGetValue(id, out CustomCharacterVoice voice);
        return voice;
    }
}