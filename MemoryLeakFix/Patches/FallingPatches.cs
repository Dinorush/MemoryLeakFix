using Gear;
using HarmonyLib;
using MemoryLeakFix.Handler;

namespace MemoryLeakFix.Patches
{
    [HarmonyPatch]
    internal static class FallingPatches
    {
        [HarmonyPatch(typeof(GlueClusterGrenadeInstance), nameof(GlueClusterGrenadeInstance.Start))]
        [HarmonyPostfix]
        private static void GlueGrenadeSpawn(GlueClusterGrenadeInstance __instance)
        {
            FallingObjectHandler.AddObject(__instance.gameObject);
        }

        [HarmonyPatch(typeof(GlowstickInstance), nameof(GlowstickInstance.Setup))]
        [HarmonyPostfix]
        private static void GlowstickSpawn(GlowstickInstance __instance)
        {
            FallingObjectHandler.AddObject(__instance.gameObject);
        }

        [HarmonyPatch(typeof(FogRepellerInstance), nameof(FogRepellerInstance.Start))]
        [HarmonyPostfix]
        private static void FogRepellerSpawn(FogRepellerInstance __instance)
        {
            FallingObjectHandler.AddObject(__instance.gameObject);
        }

        [HarmonyPatch(typeof(GlueGunProjectile), nameof(GlueGunProjectile.Awake))]
        [HarmonyPostfix]
        private static void GlueGunSpawn(GlueGunProjectile __instance)
        {
            FallingObjectHandler.AddObject(__instance.gameObject, (go) => ProjectileManager.WantToDestroyGlue(go.GetComponent<GlueGunProjectile>().SyncID), () => __instance.m_sound == null);
        }

        [HarmonyPatch(typeof(BulletWeapon), nameof(BulletWeapon.DropMagazine))]
        [HarmonyWrapSafe]
        [HarmonyPostfix]
        private static void DropMag(BulletWeapon __instance)
        {
            var pool = __instance.m_magDropPool;
            if (pool == null) return;

            FallingObjectHandler.AddPool(pool);
            var go = pool.m_freeInstances.Count > 0 ? pool.m_freeInstances.First.Value : pool.m_usedInstances.First.Value;
            FallingObjectHandler.AddObject(go, pool.Return);
        }
    }
}
