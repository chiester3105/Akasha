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
        

        public Dictionary<FactionHQ, List<PlayerSavedData>> savedData = new Dictionary<FactionHQ, List<PlayerSavedData>>();
        public void Initialize()
        {
            _eventBus.Subscribe<PlayerDisconnectedEvent>(Save);
            _eventBus.Subscribe<MissionLoadedEvent>(Clear);
        }
       
        public List<PlayerSavedData> GetSavedPlayersByHQ(FactionHQ HQ)
        {
            List<PlayerSavedData> data = savedData.TryGetValue(HQ, out var savedPlayers) ? savedPlayers.OrderByDescending(p => p.Score).ToList() : new List<PlayerSavedData>();
            return data;
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
            savedData.Clear();
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


                int index = savedData.ContainsKey(player.HQ)
                           ? savedData[player.HQ].FindIndex(x => x.SteamID == player.SteamID)
                           : -1;

                if (index != -1 && player.HQ != null)
                {
                    savedData[player.HQ][index] = data;
                }
                else
                {
                    if (!savedData.ContainsKey(player.HQ))
                    {
                        savedData.Add(player.HQ, new List<PlayerSavedData>());
                    }
                    savedData[player.HQ].Add(data);
                }
            }
            catch (Exception ex)
            {
                AkashaPlugin.Logger.LogError($"Exception while saving player data: {ex.Message}");
            }
        }
    }
}
