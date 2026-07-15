using System;
using System.Collections.Generic;
using System.Linq;
using NuclearOption.Networking;
using Steamworks;
using UnityEngine;

namespace Akasha.Data
{
    public class Sortie
    {
        public Aircraft aircraft {  get; private set; }
        public DateTime sortieStartTime { get; private set; }
        public DateTime sortieEndTime { get; private set; }
        public int sortieIdx { get; private set; }

        public TimeSpan elapsed;

        public EndReasons _sortieEndReason;

        private bool _isSortieEnded = false;

        public List<UnitInfo> killedUnits = new List<UnitInfo>();

        public float explodedNukes { get; private set; }

        public PlayerAircraftInfo selfInfo;

        public SavedPlayerData savedPlayerData;

        public float jammingAmount = 0f;

        public int detectedTargets = 0;

        public Sortie(Aircraft aircraft, int sortieSerialNumber) 
        {
            this.aircraft = aircraft;
            this.sortieIdx = sortieSerialNumber;
            sortieStartTime = DateTime.UtcNow;
            SetSelfInfo();
        }

        public void AddJamming()
        {
            jammingAmount += Time.deltaTime;
        }

        public void AddDetectedTarget()
        {
            detectedTargets++;
        }

        public void AddNuke()
        {
            explodedNukes++;
        }
        public override string ToString()
        {
            return base.ToString();
        }

        public void RegisterSortieEnd(EndReasons sortieEndReason)
        {
            if (!_isSortieEnded) 
            {
                _sortieEndReason = sortieEndReason;
                sortieEndTime = DateTime.UtcNow;
                _isSortieEnded = true;
                elapsed = sortieEndTime - sortieStartTime;
            }
        }

        public void AddKill(PersistentUnit unit, string weaponName = null)
        {
            if (aircraft.NetworkHQ == unit.unit.NetworkHQ) { return; }
            UnitInfo unitInfo = CopyUnitInfo(unit.unit);
            if (weaponName != null) unitInfo.SetKillWeapon(weaponName);
            killedUnits.Add(unitInfo);
        }
      
        public void DetectKilled(PersistentID killerPersistentID)
        {
            PersistentUnit persistentUnit;
            if (UnitRegistry.TryGetPersistentUnit(killerPersistentID, out persistentUnit))
            {
                Unit unit = persistentUnit.unit;
                if (unit is Aircraft)
                {
                    Aircraft aircraft = (Aircraft)unit;
                    if (aircraft.Player != null)
                    {
                        RegisterSortieEnd(EndReasons.killedByPlayer);
                        return;
                    }
                }
                RegisterSortieEnd(EndReasons.killedByBot);
            }
            else
            {
                RegisterSortieEnd(EndReasons.crashed);
            }
            
        }

        private UnitInfo CopyUnitInfo(Unit unit)
        {
            if (unit is Aircraft)
            {
                Aircraft aircraft = (Aircraft)unit;
                PlayerAircraftInfo aircraftInfo = new PlayerAircraftInfo();
                if (aircraft.Player != null)
                {
                    aircraftInfo.CopyPlayerInfo(aircraft);
                }
                else
                {
                    aircraftInfo.CopyUnitInfo(unit);
                }
                return aircraftInfo;
            }
            else
            {
                UnitInfo unitInfo = new UnitInfo();
                unitInfo.CopyUnitInfo(unit);
                return unitInfo;
            }
        }

        private void SetSelfInfo()
        {
            selfInfo = (PlayerAircraftInfo)CopyUnitInfo(aircraft);    
        }
         
    }
}
