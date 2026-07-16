using System;
using System.Linq;
using Akasha.Contracts;
using Akasha.Events;
using Akasha.Infrastructure;
using Akasha.Mapping;
using NuclearOption.SavedMission.ObjectiveV2.Outcomes;
using UnityEngine;

namespace Akasha.Aggregators
{
    public class MatchResultAggregator : IAggregator
    {
        private IEventBus _eventbus;
        private SortieManager _sortieManager;
        private PlayerSavedDataManager _savedDataManager;
        private IMatchResultProducer _producer;

        public MatchResultAggregator(IEventBus eventbus,
            SortieManager sortieManager,
            PlayerSavedDataManager savedDataManager,
            IMatchResultProducer producer)
        {
            _eventbus = eventbus;
            _sortieManager = sortieManager;
            _savedDataManager = savedDataManager;
            _producer = producer;
        }

        private DateTime _startTime;
        private string _mapName;
        private string _missionName;

        public void Initialize()
        {
            _eventbus.Subscribe<MissionEndedEvent>(AggregateResult);
            _eventbus.Subscribe<MissionLoadedEvent>(OnMissionLoad);
        }

        public void AggregateResult(MissionEndedEvent e)
        {
            _savedDataManager.SaveAllPlayers();
            GetWinnerAndLoser(e.declarant, e.endType, out var winner, out var loser);

            var sorties = _sortieManager.GetSorties();
            var players = _savedDataManager.GetPlayers();
            var sortiesLookup = sorties.ToLookup(s => s.selfInfo.SteamID);

            var playerRecords = sortiesLookup.CreatePlayerRecords(players);

            var record = new MatchRecord()
            {
                Winner = winner.faction.factionName,
                Duration = Time.timeSinceLevelLoad,
                StartTimeUnix = new DateTimeOffset(_startTime).ToUnixTimeSeconds(),
                MapName = _mapName,
                MissionName = _missionName,
                Players = playerRecords,
                ServerId = AkashaPlugin.ServerId,
                MatchId = Guid.NewGuid().ToString()
            };

            _producer.SendAsync(record);
        }

        // this will be broken only if devs rename/add factions
        private void GetWinnerAndLoser(FactionHQ declarant, EndType endType,
            out FactionHQ winner, out FactionHQ loser)
        {
            winner = null;
            loser = null;

            if (endType == EndType.Victory)
            {
                if (declarant.faction.factionName == "Primeva")
                {
                    winner = FactionRegistry.HqFromName("Primeva");
                    loser = FactionRegistry.HqFromName("Boscali");
                }
                else
                {
                    winner = FactionRegistry.HqFromName("Boscali");
                    loser = FactionRegistry.HqFromName("Primeva");
                }
            }
            else
            {
                if (declarant.faction.factionName == "Primeva")
                {
                    winner = FactionRegistry.HqFromName("Boscali");
                    loser = FactionRegistry.HqFromName("Primeva");
                }
                else
                {
                    winner = FactionRegistry.HqFromName("Primeva");
                    loser = FactionRegistry.HqFromName("Boscali");
                }
            }
        }

        private void OnMissionLoad(MissionLoadedEvent e)
        {
            _startTime = DateTime.Now;
            _missionName = e.missionName;
            _mapName = e.mapName;
        }
    }
}

    