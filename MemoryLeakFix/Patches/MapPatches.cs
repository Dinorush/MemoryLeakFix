using BepInEx.Unity.IL2CPP.Utils.Collections;
using CellMenu;
using HarmonyLib;
using System.Collections;
using UnityEngine;

namespace MemoryLeakFix.Patches
{
    [HarmonyPatch]
    internal static class MapPatches
    {
        public static void OnCleanup() => _routineActive = false;

        private const float UpdateInterval = 0.25f;
        private static float _nextAllowedTime = 0f;
        private static bool _routineActive = false;
        private static bool _forceAllow = false;

        [HarmonyPatch(typeof(CM_PageMap), nameof(CM_PageMap.UpdatePlayerData))]
        [HarmonyPrefix]
        private static bool Pre_UpdateData(CM_PageMap __instance)
        {
            if (_forceAllow)
            {
                _forceAllow = false;
                return true;
            }

            if (_routineActive) return false;

            float time = Clock.Time;
            if (time < _nextAllowedTime)
            {
                CoroutineManager.StartCoroutine(DelayedApply(__instance).WrapToIl2Cpp());
                return false;
            }

            _nextAllowedTime = time + UpdateInterval;
            return true;
        }

        private static IEnumerator DelayedApply(CM_PageMap __instance)
        {
            _routineActive = true;
            yield return new WaitForSeconds(UpdateInterval);
            _routineActive = false;
            __instance.UpdatePlayerData();
        }

        [HarmonyPatch(typeof(CM_PageMap), nameof(CM_PageMap.OnEnable))]
        [HarmonyPrefix]
        private static void Pre_Enable()
        {
            _forceAllow = true;
        }
    }
}
