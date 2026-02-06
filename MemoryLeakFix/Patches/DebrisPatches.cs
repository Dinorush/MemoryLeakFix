using BepInEx.Unity.IL2CPP.Utils.Collections;
using HarmonyLib;
using System;
using System.Collections;
using UnityEngine;

namespace MemoryLeakFix.Patches
{
    [HarmonyPatch]
    internal static class DebrisPatches
    {
        [HarmonyPatch(typeof(DebrisSpawner), nameof(DebrisSpawner.RunSequence), new Type[] { typeof(Transform) })]
        [HarmonyPatch(typeof(DebrisSpawner), nameof(DebrisSpawner.RunSequence), new Type[] { })]
        [HarmonyPostfix]
        private static void Post_RunSequence(DebrisSpawner __instance)
        {
            foreach (var objects in __instance.m_destructionObjects)
                CoroutineManager.StartCoroutine(DelayedHide(objects).WrapToIl2Cpp());
        }

        private static IEnumerator DelayedHide(DestructionObjects objects)
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(3f, 5f));
            var go = objects.m_DestructionObject;
            var mesh = go.GetComponent<MeshCollider>();
            if (mesh != null)
            {
                mesh.enabled = false;
            }
            var rigidBody = go.GetComponent<Rigidbody>();
            if (rigidBody != null)
            {
                rigidBody.isKinematic = true;
                rigidBody.detectCollisions = false;
            }
        }
    }
}
