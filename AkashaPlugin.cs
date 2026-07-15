using BepInEx;
using HarmonyLib;
using UnityEngine;
using BepInEx.Logging;
using System;
using Akasha.WeaponLogging;
using Akasha.Events;
using Akasha.Infrastructure;
using Akasha.Aggregators;


namespace Akasha
{

    [BepInPlugin("Akasha", "com.chiester3105.akasha", "1.0.0")]

    public class AkashaPlugin : BaseUnityPlugin
    {
        public static ManualLogSource Logger { get; private set; }
        public static ShockwaveWeaponTypeStorage ShockwaveWeaponStorage = new();
        public static UnitWeaponLogStorage WeaponStorage = new();


        private void OnApplicationQuit()
        {
            AkashaPlugin.Logger.LogWarning("OnApplicationQuit");
            Debug.LogWarning("OnApplicationQuit");
        }
        
        private void Awake()
        {
            try
            {
                AkashaPlugin.Logger = base.Logger;
                var harmony = new Harmony("Akasha");

                harmony.PatchAll();
                Logger.LogInfo("Patches applied");


                var eventBus = new EventBus();
                ServiceLocator.Register<IEventBus>(eventBus);

                var sortieManager = new SortieManager(eventBus);
                sortieManager.Initialize();

                var playerDataManager = new PlayerSavedDataManager(eventBus);
                playerDataManager.Initialize();

                var matchAggregator = new MatchResultAggregator(eventBus, sortieManager, playerDataManager);
                sortieManager.Initialize();

                Logger.LogInfo("Plugin loaded");
            }
            catch (Exception ex)
            {
                AkashaPlugin.DebugLog($"awake failed: {ex.Message}");
            }
        }

        public static void DebugLog(string message)
        {

        }
    }
}
