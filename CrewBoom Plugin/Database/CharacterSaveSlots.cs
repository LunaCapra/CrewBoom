using BepInEx.Logging;
using CrewBoom.Data;
using Reptile;
using System;
using System.Collections.Generic;
using System.IO;

namespace CrewBoom
{
    public class CharacterSaveSlots
    {
        public const int SAVE_SLOT_COUNT = 4;
        public const string SLOT_FILE_EXTENSION = ".cbs";
        public const string SAVE_FILE_EXTENSION = ".cbc";
        public static readonly string SAVE_PATH = Path.Combine(BepInEx.Paths.ConfigPath, PluginInfo.PLUGIN_NAME, "saves");

        public static CharacterSaveSlot CurrentSaveSlot;
        public static int CurrentSaveSlotId { get; private set; } = -1;
        private static Dictionary<Guid, CharacterProgress> _progressLookup = new Dictionary<Guid, CharacterProgress>();

        public static ManualLogSource DebugLog = BepInEx.Logging.Logger.CreateLogSource(PluginInfo.PLUGIN_NAME + " Saves");

        public static void LoadSlot(int slot)
        {
            if (!Directory.Exists(SAVE_PATH))
            {
                Directory.CreateDirectory(SAVE_PATH);
                return;
            }

            string slotPath = Path.Combine(SAVE_PATH, slot.ToString() + SLOT_FILE_EXTENSION);
            if (File.Exists(slotPath))
            {
                try
                {
                    using (FileStream stream = File.OpenRead(slotPath))
                    {
                        using (BinaryReader reader = new BinaryReader(stream))
                        {
                            CurrentSaveSlot.Read(reader);
                        }
                    }
                }
                catch (Exception)
                {
                    DebugLog.LogWarning($"Failed to read slot data for slot {slot}");
                }

                DebugLog.LogMessage($"Loaded custom character save slot {slot}");
            }

            CurrentSaveSlotId = slot;
        }
        public static void SaveSlot()
        {
            if (CurrentSaveSlotId == -1)
            {
                return;
            }

            if (!Directory.Exists(SAVE_PATH))
            {
                Directory.CreateDirectory(SAVE_PATH);
            }

            string slotPath = Path.Combine(SAVE_PATH, CurrentSaveSlotId.ToString() + SLOT_FILE_EXTENSION);
            try
            {
                using (FileStream stream = File.OpenWrite(slotPath))
                {
                    using (BinaryWriter writer = new BinaryWriter(stream))
                    {
                        CurrentSaveSlot.Write(writer);
                    }
                }
            }
            catch (Exception)
            {
                DebugLog.LogError($"Failed to write slot data for slot {CurrentSaveSlotId}");
            }
        }

        public static bool GetCharacterData(Guid guid, out CharacterProgress progress)
        {
            //Get the data from the lookup, otherwise load it
            if (_progressLookup.TryGetValue(guid, out progress))
            {
                return true;
            }
            else
            {
                progress = new();
            }

            if (CurrentSaveSlotId == -1)
            {
                LogUninitialized();
                return false;
            }

            if (!EnsureSlotDirectory(out string slotPath))
            {
                return false;
            }

            string characterFilePath = CharacterFilePath(slotPath, guid);
            if (!File.Exists(characterFilePath))
            {
                if (CharacterDatabase.GetCharacter(guid, out CustomCharacter customCharacter))
                {
                    progress = new()
                    {
                        outfit = 0,
                        moveStyle = (MoveStyle)customCharacter.Definition.DefaultMovestyle,
                        moveStyleSkin = 0
                    };
                    _progressLookup.Add(guid, progress);
                    return true;
                }
                return false;
            }

            try
            {
                using (FileStream stream = File.OpenRead(characterFilePath))
                {
                    using (BinaryReader reader = new BinaryReader(stream))
                    {
                        progress.Read(reader);
                    }
                }

                _progressLookup.Add(guid, progress);
                return true;
            }
            catch (Exception)
            {
                DebugLog.LogError($"Could not load character save data for character with GUID \"{guid}\".");
            }

            return false;
        }
        public static void SaveCharacterData(Guid guid)
        {
            if (CurrentSaveSlotId == -1)
            {
                LogUninitialized();
                return;
            }

            EnsureSlotDirectory(out string slotPath);

            if (!_progressLookup.TryGetValue(guid, out CharacterProgress progress))
            {
                DebugLog.LogError($"No save data made for character with GUID \"{guid}\".");
                return;
            }

            string characterFilePath = CharacterFilePath(slotPath, guid);
            try
            {
                using (FileStream stream = File.OpenWrite(characterFilePath))
                {
                    using (BinaryWriter writer = new BinaryWriter(stream))
                    {
                        progress.Write(writer);
                    }
                }
            }
            catch (Exception)
            {
                DebugLog.LogError($"Could not write character save data for character with GUID \"{guid}\".");
            }
        }

        private static bool EnsureSlotDirectory(out string slotPath)
        {
            bool alreadyExisted = true;

            if (!Directory.Exists(SAVE_PATH))
            {
                Directory.CreateDirectory(SAVE_PATH);
                alreadyExisted = false;
            }
            slotPath = Path.Combine(SAVE_PATH, CurrentSaveSlotId.ToString());
            if (!Directory.Exists(slotPath))
            {
                Directory.CreateDirectory(slotPath);
                alreadyExisted = false;
            }

            return alreadyExisted;
        }
        private static string CharacterFilePath(string slotPath, Guid guid)
        {
            return Path.Combine(slotPath, guid.ToString() + SAVE_FILE_EXTENSION);
        }
        private static void LogUninitialized()
        {
            DebugLog.LogWarning("Can't read or write character data before slot was initialized!");
        }
    }

    public struct CharacterSaveSlot
    {
        public Guid LastPlayedCharacter;

        public void Write(BinaryWriter writer)
        {
            writer.Write(LastPlayedCharacter.ToString());
        }
        public void Read(BinaryReader reader)
        {
            Guid.TryParse(reader.ReadString(), out LastPlayedCharacter);
        }
    }
}