using HarmonyLib;
using System;
using UnityEngine;

namespace MemoryLeakFix.Patches
{
    [HarmonyPatch]
    internal static class IRFRenderPatches
    {
        public static Camera DrawMeshCamera = null!;

        [HarmonyPatch(typeof(Graphics), nameof(Graphics.DrawMeshInstancedIndirect), new Type[]
            {
                typeof(Mesh),
                typeof(int),
                typeof(Material),
                typeof(Bounds),
                typeof(ComputeBuffer),
                typeof(int),
                typeof(MaterialPropertyBlock),
                typeof(UnityEngine.Rendering.ShadowCastingMode),
                typeof(bool),
                typeof(int),
                typeof(Camera),
                typeof(UnityEngine.Rendering.LightProbeUsage),
                typeof(LightProbeProxyVolume)
            }
        )]
        [HarmonyPrefix]
        private static void FixCamera(ref Camera camera)
        {
            // Only called by IRFs with null camera, so safe to always replace
            camera = DrawMeshCamera;
        }

        [HarmonyPatch(typeof(PreLitVolume), nameof(PreLitVolume.Setup))]
        [HarmonyPostfix]
        private static void SetCamera(PreLitVolume __instance)
        {
            DrawMeshCamera = __instance.m_camera;
        }
    }

}
