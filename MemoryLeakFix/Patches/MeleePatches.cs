using Expedition;
using GameData;
using Gear;
using HarmonyLib;
using UnityEngine;

namespace MemoryLeakFix.Patches
{
    [HarmonyPatch]
    internal static class MeleePatches
    {
        private static SNetwork.SNet_BroadcastAction<pMeleeHitPacketStruct>? s_meleePacket;
        [HarmonyPatch(typeof(MeleeWeaponFirstPerson), nameof(MeleeWeaponFirstPerson.Setup))]
        [HarmonyPrefix]
        private static bool FixPacket(MeleeWeaponFirstPerson __instance, ItemDataBlock data)
        {
            if (s_meleePacket != null)
            {
                __instance.ItemDataBlock = data;
                __instance.PublicName = data.publicName;
                __instance.m_expeditionGearComponent = __instance.GetComponent<ExpeditionGear>();
                if (__instance.m_expeditionGearComponent != null)
                    __instance.m_expeditionGearComponent.Setup();
                __instance.Sound = new CellSoundPlayer();
                __instance.SightLookAlign = __instance.transform.FindChildRecursive(__instance.ItemDataBlock.SightLookAlign);
                __instance.MuzzleAlign = __instance.transform.FindChildRecursive(__instance.ItemDataBlock.MuzzleAlign);
                __instance.BackpackAlign = __instance.transform.FindChildRecursive(__instance.ItemDataBlock.BackpackAlign);
                __instance.enabled = false;
                __instance.ModelData = __instance.GetComponentInChildren<MeleeWeaponModelData>();
                __instance.WeaponAnimator = __instance.ModelData.m_animator;
                __instance.m_hitPacket = s_meleePacket;
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(MeleeWeaponFirstPerson), nameof(MeleeWeaponFirstPerson.Setup))]
        [HarmonyPostfix]
        private static void CachePacket(MeleeWeaponFirstPerson __instance)
        {
            s_meleePacket = __instance.m_hitPacket;
        }
    }
}
