using HarmonyLib;

namespace MemoryLeakFix.Patches
{
    [HarmonyPatch]
    internal static class SoundPatches
    {
        [HarmonyPatch(typeof(GlueGunProjectile), nameof(GlueGunProjectile.SyncDestroy))]
        [HarmonyPrefix]
        private static void GlueGunSpawn(GlueGunProjectile __instance)
        {
            __instance.m_sound.Recycle();
        }

        [HarmonyPatch(typeof(ProjectileBase), nameof(ProjectileBase.Collision))]
        [HarmonyPostfix]
        private static void ProjectileDestroy(ProjectileBase __instance)
        {
            __instance.m_soundPlayer.Recycle();
        }

        [HarmonyPatch(typeof(StrikerBigTentacle), nameof(StrikerBigTentacle.OnDead))]
        [HarmonyPostfix]
        private static void BigTentacleDead(StrikerBigTentacle __instance)
        {
            __instance.m_tipSound.Recycle();
        }
    }
}
