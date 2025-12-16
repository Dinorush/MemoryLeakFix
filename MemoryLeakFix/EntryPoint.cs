using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using MemoryLeakFix.Handler;

namespace MemoryLeakFix
{
    [BepInPlugin("Dinorush." + MODNAME, MODNAME, "1.3.0")]
    [BepInDependency("dev.gtfomodding.gtfo-api", BepInDependency.DependencyFlags.HardDependency)]
    internal sealed class EntryPoint : BasePlugin
    {
        public const string MODNAME = "MemoryLeakFix";

        public override void Load()
        {
            new Harmony(MODNAME).PatchAll();
            Log.LogMessage("Loaded " + MODNAME);

            GTFO.API.LevelAPI.OnLevelCleanup += OnLevelCleanup;
            GTFO.API.AssetAPI.OnStartupAssetsLoaded += OnAssetsLoaded;
        }

        private void OnLevelCleanup()
        {
            if (Decay.s_poolHandle != null)
                Decay.s_poolHandle.Clear();
            FallingObjectHandler.Clear();
        }

        private void OnAssetsLoaded()
        {
            FallingObjectHandler.Current.EnsureInit();
        }
    }
}