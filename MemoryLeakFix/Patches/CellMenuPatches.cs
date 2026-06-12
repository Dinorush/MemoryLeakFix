using CellMenu;
using HarmonyLib;

namespace MemoryLeakFix.Patches
{
    internal static class CellMenuPatches
    {
        [HarmonyPatch(typeof(CM_ScrollWindow), nameof(CM_ScrollWindow.ResetHeaders))]
        [HarmonyPostfix]
        private static void Post_ResetHeaders(CM_ScrollWindow __instance)
        {
            // Never cleared, thus adds duplicates every time the window is opened
            __instance.m_nonContentItems?.Clear();
        }
    }
}
