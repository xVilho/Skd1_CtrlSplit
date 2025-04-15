using MelonLoader;
using HarmonyLib;
using UnityEngine;
using Il2CppScheduleOne.UI;
using Il2CppScheduleOne.UI.Items;
using Il2CppScheduleOne.ItemFramework;

[assembly: MelonInfo(typeof(CtrlSplit.CtrlSplit), "CtrlSplit", "1.0", "xVilho")]
[assembly: MelonColor(255, 200, 150, 255)]

namespace CtrlSplit
{
    public class CtrlSplit : MelonMod
    {
        private static ItemSlot cachedSlot = null;
        private static bool rightClickHeld = false;
        private static MelonLogger.Instance Logger;

        public override void OnInitializeMelon()
        {
            Logger = LoggerInstance; // capture for static use
            Logger.Msg("✅ initialized");
            HarmonyInstance.PatchAll(typeof(CtrlSplit));
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(ItemUIManager), "Update")]
        private static void PostfixUpdate(ItemUIManager __instance)
        {
            // Cache hovered slot on right-click down
            if (Input.GetMouseButtonDown(1))
            {
                cachedSlot = __instance.HoveredSlot?.assignedSlot;
                rightClickHeld = true;
            }
            else if (Input.GetMouseButtonUp(1))
            {
                rightClickHeld = false;
                cachedSlot = null;
            }

            // Check Ctrl + RightClick + Scroll
            bool ctrlHeld = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
            if (!ctrlHeld || !rightClickHeld)
                return;

            float scroll = Input.mouseScrollDelta.y;
            if (Mathf.Abs(scroll) < 0.01f)
                return;

            int currentAmount = __instance.draggedAmount;
            if (currentAmount <= 0)
                return;

            int scrollDirection = scroll > 0f ? 1 : -1;
            int step = 5;
            int maxAmount = 999;

            // Get true max amount from ItemData
            var instance = cachedSlot?.ItemInstance;
            if (instance != null)
            {
                var data = instance.GetItemData();
                if (data != null)
                    maxAmount = Mathf.Max(data.Quantity - 1, 1); // Leave 1 behind
            }

            int newAmount;
            if (scrollDirection > 0)
            {
                newAmount = currentAmount == 1
                    ? step
                    : Mathf.Min(((currentAmount - 1) / step + 1) * step, maxAmount);
            }
            else
            {
                newAmount = currentAmount <= step
                    ? 0
                    : Mathf.Max(((currentAmount - 1) / step) * step - step, 1);
            }

            __instance.SetDraggedAmount(newAmount);
            Logger.Msg($"Ctrl+Mb1+Scroll | {currentAmount} → {newAmount} (max: {maxAmount + 1})");
        }
    }
}
