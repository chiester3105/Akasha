using System;
using System.Collections.Generic;
using Akasha.Data;
using NuclearOption.Networking;
using System.Linq;
using Akasha.Events;

namespace Akasha.Aggregators
{
    public class PlayerSavedDataManager : IAggregator
    {
        private readonly IEventBus _eventBus;

        public PlayerSavedDataManager(IEventBus eventBus) => _eventBus = eventBus;
        
        private Dictionary<ulong,  PlayerSavedData> _savedData = new();
        public void Initialize()
        {
            _eventBus.Subscribe<PlayerDisconnectedEvent>(Save);
            _eventBus.Subscribe<MissionLoadedEvent>(Clear);
        }
       
        public List<PlayerSavedData> GetPlayers()
        {
            return _savedData.Values.ToList();
        }
        public void SaveAllPlayers()
        {
            foreach (Faction faction in FactionRegistry.factions)
            {
                FactionHQ HQ = FactionRegistry.HQFromFaction(faction);
                foreach (Player player in HQ.GetPlayers(false))
                {
                    Save(player);
                }
            }
        }

        public void Clear(MissionLoadedEvent e)
        {
            _savedData.Clear();
        }
        private void Save(PlayerDisconnectedEvent e)
        {
            if (!e.player.TryGetPlayer<Player>(out var player))
            {
                AkashaPlugin.Logger.LogError("Player object disposed before saving");
            }
            else
                Save(player);          
        }

        public void Save(Player player)
        {
            try
            {
                PlayerSavedData data = new PlayerSavedData(
                    player.GetAuthData().SteamID.m_SteamID,
                    player.PlayerName,
                    player.PlayerScore,
                    player.HQ
                );

                _savedData[data.SteamID] = data;
            }
            catch (Exception ex)
            {
                AkashaPlugin.Logger.LogError($"Exception while saving player data: {ex.Message}");
            }
        }
    }
}
