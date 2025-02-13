using Gear;
using HarmonyLib;
using MemoryLeakFix.Handler;

namespace MemoryLeakFix.Patches
{
    [HarmonyPatch]
    internal static class FallingPatches
    {
        [HarmonyPatch(typeof(GlueGunProjectile), nameof(GlueGunProjectile.Awake))]
        [HarmonyPostfix]
        private static void GlueGunSpawn(GlueGunProjectile __instance)
        {
            FallingObjectHandler.AddObject(__instance.gameObject, (go) => ProjectileManager.WantToDestroyGlue(go.GetComponent<GlueGunProjectile>().SyncID));
        }

        [HarmonyPatch(typeof(BulletWeapon), nameof(BulletWeapon.DropMagazine))]
        [HarmonyPostfix]
        private static void DropMag(BulletWeapon __instance)
        {
            var pool = __instance.m_magDropPool;
            var go = pool.m_freeInstances.Count > 0 ? pool.m_freeInstances.First.Value : pool.m_usedInstances.First.Value;
            FallingObjectHandler.AddObject(go, pool.Return);
        }
    }
}
