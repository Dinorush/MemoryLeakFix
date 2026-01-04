using BepInEx.Unity.IL2CPP.Utils.Collections;
using CellMenu;
using HarmonyLib;
using System.Collections;
using UnityEngine;

namespace MemoryLeakFix.Patches
{
    [HarmonyPatch]
    internal static class DisinfectPatches
    {
        public static void OnCleanup() => _routineActive = false;

        private const float UpdateInterval = 0.5f;
        private static float _nextAllowedTime = 0f;
        private static bool _routineActive = false;

        [HarmonyPatch(typeof(CM_PageMap), nameof(CM_PageMap.UpdatePlayerData))]
        [HarmonyPrefix]
        private static bool Pre_UpdateData(CM_PageMap __instance)
        {
            if (_routineActive) return false;

            float time = Time.realtimeSinceStartup;
            if (time < _nextAllowedTime)
            {
                _routineActive = true;
                CoroutineManager.StartCoroutine(DelayedApply(__instance).WrapToIl2Cpp());
                return false;
            }

            _nextAllowedTime = time + UpdateInterval;
            return true;
        }

        private static IEnumerator DelayedApply(CM_PageMap __instance)
        {
            yield return new WaitForSeconds(UpdateInterval);
            _routineActive = false;
            __instance.UpdatePlayerData();
        }
    }
}
