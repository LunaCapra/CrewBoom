

using HarmonyLib;

namespace CrewBoom.Patches
{
    [HarmonyPatch(typeof(Reptile.SaveSlotHandler), nameof(Reptile.SaveSlotHandler.SetCurrentSaveSlotDataBySlotId))]
    public class SaveSlotHandlerLoadPatch
    {
        public static void Postfix(int saveSlotId)
        {
            CharacterSaveSlots.LoadSlot(saveSlotId);
        }
    }
}
