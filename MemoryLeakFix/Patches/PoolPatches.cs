using HarmonyLib;
using MemoryLeakFix.Utils;
using UnityEngine;

namespace MemoryLeakFix.Patches
{
    [HarmonyPatch]
    internal static class PoolPatches
    {
        [HarmonyPatch(typeof(ObjectPooler.Pool), nameof(ObjectPooler.Pool.Clear))]
        [HarmonyWrapSafe]
        [HarmonyPrefix]
        private static void DestroyBeforeClear(ObjectPooler.Pool __instance)
        {
            foreach (var go in __instance.m_freeInstances)
                GameObject.Destroy(go);
            foreach (var go in __instance.m_usedInstances)
                GameObject.Destroy(go);
        }
    }
}
