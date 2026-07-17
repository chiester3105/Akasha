using BepInEx;
using HarmonyLib;
using UnityEngine;
using BepInEx.Logging;
using System;
using Akasha.WeaponLogging;
using Akasha.Events;
using Akasha.Infrastructure;
using Akasha.Aggregators;
using Akasha.Infrastructure.Kafka;

namespace Akasha
{
    [BepInPlugin("Akasha", "com.chiester3105.akasha", "1.0.0")]

    public class AkashaPlugin : BaseUnityPlugin
    {
        public static ManualLogSource Logger { get; private set; }
        public static ShockwaveWeaponTypeStorage ShockwaveWeaponStorage = new();
        public static UnitWeaponLogStorage WeaponStorage = new();

        //inject from env vars 
        public static string ServerId { get; private set; }
        public static string MessagesPath { get; private set; }
        private void OnApplicationQuit()
        {
            AkashaPlugin.Logger.LogWarning("OnApplicationQuit");
            Debug.LogWarning("OnApplicationQuit");
        }
        
        private void Awake()
        {
            try
            {
                var harmony = new Harmony("Akasha");
                harmony.PatchAll();

                AkashaPlugin.Logger = base.Logger;
                Logger.LogInfo("Patches applied");

                ServerId = Config.Bind<string>("Enviroment", "ServerId", "SERVER", "Unique server id").Value;
                MessagesPath = Config.Bind<string>("Enviroment", "Messages path", "/fallbacks", "Path for messages that failed to send").Value;

                var eventBus = new EventBus();
                ServiceLocator.Register<IEventBus>(eventBus);

                var sortieManager = new SortieManager(eventBus);
                sortieManager.Initialize();

                var playerDataManager = new PlayerSavedDataManager(eventBus);
                playerDataManager.Initialize();

                var bootstrapServers = Config.Bind<string>("Kafka", "BootstrapServers", "localhost:9092", "Kafka bootstrap server").Value;
                var topic = Config.Bind<string>("Kafka", "Topic", "match-results", "Kafka topic for match results").Value;
                var producer = new KafkaMatchResultProducer(bootstrapServers, topic, ServerId);


                var matchAggregator = new MatchResultAggregator(eventBus, sortieManager, playerDataManager, producer);
                matchAggregator.Initialize();

                Logger.LogInfo("Akasha loaded successfuly.");
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
