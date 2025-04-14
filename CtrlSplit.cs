using MelonLoader;
using HarmonyLib;
using UnityEngine;
using Il2CppScheduleOne.UI;
using Il2CppScheduleOne.UI.Items;

[assembly: MelonInfo(typeof(RightClickSplitMod.RightClickSplitMod), "RightClickSplitMod", "0.1.0", "xVilho")]
[assembly: MelonColor(255, 200, 150, 255)]

namespace RightClickSplitMod
{
    public class RightClickSplitMod : MelonMod
    {
        public override void OnInitializeMelon()
        {
            HarmonyInstance.PatchAll(typeof(RightClickSplitMod));
            MelonLogger.Msg("✅ RightClickSplitMod initialized with proper scroll handling!");
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(ItemUIManager), "Update")]
        private static void PostfixUpdate(ItemUIManager __instance)
        {
            bool ctrlHeld = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
            if (!ctrlHeld)
                return;

            if (!Input.GetMouseButton(1))
                return;

            float scroll = Input.mouseScrollDelta.y;
            if (Mathf.Abs(scroll) < 0.01f)
                return;

            int currentAmount = __instance.draggedAmount;
            if (currentAmount <= 0)
                return;

            int scrollDirection = scroll > 0f ? 1 : -1;

            // Define step size
            int step = 5;
            int maxAmount = 59;

            int newAmount;

            if (scrollDirection > 0)
            {
                // Scroll up: next multiple of 5
                if (currentAmount == 1)
                    newAmount = 5;
                else
                    newAmount = Mathf.Min(((currentAmount - 1) / step + 1) * step, maxAmount);
            }
            else
            {
                // Scroll down: previous multiple of 5, but minimum is 1
                if (currentAmount <= step)
                    newAmount = 1;
                else
                    newAmount = Mathf.Max(((currentAmount - 1) / step) * step - step, 1);
            }

            __instance.SetDraggedAmount(newAmount);

            MelonLogger.Msg($"[RightClickSplitMod] Ctrl held | Scroll: {scrollDirection} | Amount: {currentAmount} → {newAmount}");
        }

    }
}

