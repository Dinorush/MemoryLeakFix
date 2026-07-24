using HarmonyLib;
using Player;
using System;
using System.Collections.Generic;
using UnityEngine;
using WindVolume;

namespace MemoryLeakFix.Patches
{
    [HarmonyPatch]
    internal class WindVolumePatches
    {
        private const int CacheLen = 64;
        private readonly static List<WindVolumeAffector> _excluded = new();
        private readonly static List<WindVolumeAffector> _affectors = new();

        [HarmonyPatch(typeof(WindVolumeCamera), nameof(WindVolumeCamera.UpdateVolume))]
        [HarmonyWrapSafe]
        [HarmonyPrefix]
        private static void CaptureOverflow(WindVolumeCamera __instance)
        {
            var instances = WindVolumeAffector.instances;
            if (instances.Count < CacheLen) return;

            Span<int> counts = stackalloc[] { 0, 0, 0, 0 };
            __instance.EnsureInitialized();
            Bounds bounds = new(__instance.VolumeCenter, __instance.VolumeSize);
            bool playing = Application.isPlaying;
            var dimension = PlayerManager.GetLocalPlayerAgent().DimensionIndex;

            foreach (var instance in instances)
            {
                var shape = instance.Shape;
                var intShape = (int)shape;
                if (counts[intShape] == CacheLen)
                {
                    _excluded.Add(instance);
                    continue;
                }

                if (shape == AffectorShape.Global)
                {
                    if (playing && dimension != instance.DimensionIndex)
                    {
                        _excluded.Add(instance);
                        continue;
                    }
                }
                else if (!bounds.Intersects(instance.Bounds))
                {
                    _excluded.Add(instance);
                    continue;
                }

                counts[intShape]++;
                _affectors.Add(instance);
            }

            if (_affectors.Count > _excluded.Count / 2)
            {
                foreach (var affector in _excluded)
                    instances.Remove(affector);
            }
            else
            {
                instances.Clear();
                foreach (var affector in _affectors)
                    instances.Add(affector);
            }
            _affectors.Clear();
        }

        [HarmonyPatch(typeof(WindVolumeCamera), nameof(WindVolumeCamera.UpdateVolume))]
        [HarmonyWrapSafe]
        [HarmonyPostfix]
        private static void RefillList()
        {
            if (_excluded.Count > 0)
            {
                var instances = WindVolumeAffector.instances;
                foreach (var affector in _excluded)
                    instances.Add(affector);
                _excluded.Clear();
            }
        }
    }
}
