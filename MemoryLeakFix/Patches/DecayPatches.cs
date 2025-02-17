using UnityEngine;
using System;
using HarmonyLib;
using System.Collections;
using BepInEx.Unity.IL2CPP.Utils.Collections;

namespace MemoryLeakFix.Patches
{
    [HarmonyPatch]
    internal static class DecayPatches
    {
        [HarmonyPatch(typeof(Decay), nameof(Decay.Initialize), new Type[] {typeof(SkinnedMeshRenderer), typeof(Il2CppSystem.Collections.Generic.List<IRF.InstancedRenderFeature>)})]
        [HarmonyPostfix]
        private static void AddEndCallback(Decay __instance)
        {
            __instance.OnDecaySafeToDespawnRenderer += (Action)(() =>
            {
                __instance.StartCoroutine(DelayedClear(__instance).WrapToIl2Cpp());
            });
        }

        // For some reason get nullrefs on shadows if this is done immediately, so have to delay by some duration.
        // Seems to be ~1-1.5s, setting higher for safety. Not critical to do immediately.
        private const float DecayClearDelay = 5f;
        private static IEnumerator DelayedClear(Decay __instance)
        {
            yield return new WaitForSeconds(DecayClearDelay);
            if (__instance.m_particles != null)
                __instance.m_particles.Stop(withChildren: true, ParticleSystemStopBehavior.StopEmittingAndClear);
            __instance.m_playing = true;
        }
    }
}
