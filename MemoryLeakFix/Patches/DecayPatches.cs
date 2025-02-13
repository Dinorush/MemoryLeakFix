using UnityEngine;
using System;
using HarmonyLib;

namespace MemoryLeakFix.Patches
{
    [HarmonyPatch]
    internal static class DecayPatches
    {
        [HarmonyPatch(typeof(Decay), nameof(Decay.Awake))]
        [HarmonyPostfix]
        private static void AddEndCallback(Decay __instance)
        {
            __instance.OnDecaySafeToDespawnRenderer += (Action) (() =>
            {
                // JFS - Stop particles so the check to end the decay is sure to be valid
                if (__instance.m_particles != null)
                    __instance.m_particles.Stop(withChildren: true, ParticleSystemStopBehavior.StopEmittingAndClear);
                __instance.m_playing = true;
            });
        }
    }
}
