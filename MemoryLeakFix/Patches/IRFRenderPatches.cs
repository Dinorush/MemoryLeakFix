using HarmonyLib;
using System;
using UnityEngine;

namespace MemoryLeakFix.Patches
{
    [HarmonyPatch]
    internal static class IRFRenderPatches
    {
        private static Camera _drawMeshCamera = null!;
        private static bool _overrideCamera = false;

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
            if (_overrideCamera)
                camera = _drawMeshCamera;
        }

        [HarmonyPatch(typeof(FPSCamera), nameof(FPSCamera.OnControllerEnable))]
        [HarmonyPostfix]
        private static void EnableCamera(FPSCamera __instance)
        {
            _drawMeshCamera = __instance.m_camera;
            _overrideCamera = true;
        }

        [HarmonyPatch(typeof(FPSCamera), nameof(FPSCamera.OnControllerDisable))]
        [HarmonyPostfix]
        private static void DisableCamera()
        {
            _overrideCamera = false;
        }
    }
}
