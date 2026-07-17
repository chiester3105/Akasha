using System;
using System.Collections.Generic;
using System.Linq;
using Akasha.Data;
using Akasha.Events;

namespace Akasha.Aggregators
{
    public class SortieManager : IAggregator
    {
        private readonly IEventBus _eventBus;
        public SortieManager(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        public void Initialize()
        {
            _eventBus.Subscribe<UnitRegisteredEvent>(RegisterPlayerSortieStart);
            _eventBus.Subscribe<UnitUnregisteredEvent>(RegisterPlayerSortieEnd);
            _eventBus.Subscribe<MissionLoadedEvent>(Reset);
            _eventBus.Subscribe<NuclearWarheadDetonatedEvent>(AddNuke);
            _eventBus.Subscribe<JammingEvent>(AddJammingAmmount);
            _eventBus.Subscribe<KillEvent>(AddKill);
            _eventBus.Subscribe<TargetDetectedEvent>(AddDetects);
        }

        private Dictionary<PersistentID, Sortie> playerAircraftsInfo = new Dictionary<PersistentID, Sortie>();

        public void Reset(MissionLoadedEvent e)
        {
            playerAircraftsInfo.Clear();
        }
        public void RegisterPlayerSortieStart(UnitRegisteredEvent e)
        {
            if (e.unit is Aircraft)
            {
                Aircraft aircraft = (Aircraft)e.unit;
                if (aircraft.Player != null)
                {
                    Sortie sortie = new Sortie(aircraft, playerAircraftsInfo.Count() + 1);
                    playerAircraftsInfo.Add(e.id, sortie);
                }
            }
        }

        public void RegisterPlayerSortieEnd(UnitUnregisteredEvent e)
        {
            if (e.unit is Aircraft aircraft)
            {
                PersistentID id = aircraft.persistentID;
                if (playerAircraftsInfo.TryGetValue(id, out var sortie))
                {
                    sortie.RegisterSortieEnd(EndReasons.landed);
                }
            }
        }
        public void AddDetects(TargetDetectedEvent e)
        {
            if (playerAircraftsInfo.TryGetValue(e.detector, out var sortie))
            {
                sortie.AddDetectedTarget();
            }
        }
        public void AddJammingAmmount(JammingEvent e)
        {
            if (playerAircraftsInfo.TryGetValue(e.owner, out var sortie))
            {
                sortie.AddJamming();
            }
        }
        public void AddKill(KillEvent e)
        {
            PersistentID killerPersistentID = e.killer;
            
            if(!UnitRegistry.TryGetPersistentUnit(killerPersistentID, out var persistentUnitKilled))
            {
                AkashaPlugin.Logger.LogError("THIS SHOULD NEVER HAPPEN: KILLED UNIT DOES NOT EXIST");
                return;
            }
            try
            {
                if (playerAircraftsInfo.TryGetValue(killerPersistentID, out var sortie))
                {
                    string weaponName = e.weaponName;
                    sortie.AddKill(persistentUnitKilled, weaponName);
                }
                if (persistentUnitKilled.player != null
                    && playerAircraftsInfo.TryGetValue(persistentUnitKilled.id, out var sortie2))
                {
                    sortie2.DetectKilled(killerPersistentID, e.weaponName);
                }
            }
            catch (Exception ex)
            {
                AkashaPlugin.Logger.LogError($"Fail while adding kill: {ex.Message}");
            }
        }
        public void AddNuke(NuclearWarheadDetonatedEvent e)
        {
            if(playerAircraftsInfo.TryGetValue(e.id, out var sortie))
                sortie.AddNuke();
        }

        public IEnumerable<Sortie> GetSorties()
        {
            return playerAircraftsInfo.Values;
        }
    }
}
