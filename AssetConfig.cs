using BepInEx.Configuration;
using BepInEx.Logging;
using BrcCustomCharactersLib;
using Reptile;
using System;

namespace BrcCustomCharacters
{
    public static class AssetConfig
    {
        private static ConfigEntry<string>[] _characterIdOverrides;

        public static void Init(ConfigFile config)
        {
            var values = Enum.GetValues(typeof(Characters));
            _characterIdOverrides = new ConfigEntry<string>[values.Length - 2];
            foreach (Characters character in values)
            {
                if (character == Characters.NONE || character == Characters.MAX)
                {
                    continue;
                }

                BrcNamedCharacter characterName = (BrcNamedCharacter)character;
                _characterIdOverrides[(int)character] = config.Bind<string>("Replacement IDs", $"{characterName}", null, $"Enter a GUID of a character bundle to always load for {characterName}.\nLeave blank to auto-detect."); ;
                if (_characterIdOverrides[(int)character].Value != string.Empty)
                {
                    ManualLogSource log = BepInEx.Logging.Logger.CreateLogSource("BrcCustomCharacters Config");
                    log.LogInfo($"{characterName} override: {_characterIdOverrides[(int)character].Value}");
                }
            }
        }

        public static bool GetCharacterOverride(Characters character, out Guid id)
        {
            id = Guid.Empty;

            string guidString = _characterIdOverrides[(int)character].Value;
            if (guidString == string.Empty)
            {
                return false;
            }

            id = Guid.Parse(guidString);
            if (id.Equals(Guid.Empty))
            {
                return false;
            }

            return true;
        }
    }
}
