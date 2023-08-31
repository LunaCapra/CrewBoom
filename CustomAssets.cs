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

        string[] characterNames = System.Enum.GetNames(typeof(Characters));
        _characterBundles = new AssetBundle[characterNames.Length];
        _characterGraffiti = new Material[characterNames.Length];
        _characterVoices = new Dictionary<SfxCollectionID, CustomCharacterVoice>();

        for (int i = 0; i < characterNames.Length; i++)
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
        return _characterBundles[(int)character] != null;
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