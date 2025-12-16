using Enemies;
using FX_EffectSystem;
using HarmonyLib;
using IRF;
using MemoryLeakFix.Utils;
using System.Diagnostics;
using UnityEngine;

namespace MemoryLeakFix.Patches
{
    [HarmonyPatch]
    internal static class ObjectCleanupPatches
    {
        [HarmonyPatch(typeof(InstancedRenderFeature), nameof(InstancedRenderFeature.OnDestroy))]
        [HarmonyPrefix]
        private static void Pre_Destroy(InstancedRenderFeature __instance)
        {
            if (__instance.transform.parent == null) return;

            var renderer = __instance.m_renderer;
            if (renderer != null && renderer.m_compute != null)
            {
                Object.Destroy(renderer.m_compute);
                renderer.m_compute = null;
            }

            if (__instance.m_controller != null)
            {
                var materials = __instance.m_controller.Materials;
                foreach (var mat in materials)
                    Object.Destroy(mat);
            }
        }

        [HarmonyPatch(typeof(EnemyAgent), nameof(EnemyAgent.OnDestroy))]
        [HarmonyPrefix]
        private static void Pre_EnemyDestroy(EnemyAgent __instance)
        {
            var flyerController = __instance.GetComponentInChildren<FlyerAnimationController>(includeInactive: true);
            if (flyerController != null)
                Object.Destroy(flyerController.m_fleshBulbs.m_material);
        }

        [HarmonyPatch(typeof(FX_Manager), nameof(FX_Manager.ResetManager))]
        [HarmonyPrefix]
        private static void Pre_Reset()
        {
            foreach (var light in FX_Manager.s_effectPointLights)
                Object.Destroy(light.gameObject);
            FX_Manager.s_activeLights.Clear();
            FX_Manager.s_freeLights.Clear();
        }

        [HarmonyPatch(typeof(GS_AfterLevel), nameof(GS_AfterLevel.CleanupAfterExpedition))]
        [HarmonyPriority(Priority.Low)]
        [HarmonyPostfix]
        private static void Post_AllCleanup()
        {
            // For some reason, these get generated and keep stacking the longer the game runs.
            // JFS, deleting them after ALL other cleanups have ran to reduce performance cost.
            foreach (var obj in Object.FindObjectsOfType<TextAsset>())
                if (obj.bytes.Length == 0)
                    Object.Destroy(obj);
        }
    }
}
