using BepInEx.Configuration;
using BepInEx.Logging;
using BrcCustomCharactersLib;
using Reptile;
using System;

namespace BrcCustomCharacters
{
    public static class AssetConfig
    {
        private const string CONFIG_DESCRIPTION = "Enter a GUID of a character bundle to always load for {0} (Blank = Auto-detect, \"OFF\" = Default character for you)";

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
                _characterIdOverrides[(int)character] = config.Bind<string>("Replacement IDs", characterName.ToString(), null, string.Format(CONFIG_DESCRIPTION, characterName)); ;
                if (_characterIdOverrides[(int)character].Value != string.Empty)
                {
                    ManualLogSource log = BepInEx.Logging.Logger.CreateLogSource("BrcCustomCharacters Config");
                    log.LogInfo($"{characterName} override: {_characterIdOverrides[(int)character].Value}");
                }
            }
        }

        public static bool GetCharacterOverride(Characters character, out Guid id, out bool isDisabled)
        {
            id = Guid.Empty;
            isDisabled = false;

            string guidString = _characterIdOverrides[(int)character].Value;
            if (guidString == string.Empty)
            {
                return false;
            }
            if (guidString == "OFF")
            {
                isDisabled = true;
                return false;
            }

            try
            {
                id = Guid.Parse(guidString);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
