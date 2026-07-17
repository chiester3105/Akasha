using System;
using System.Collections.Generic;
using UnityEngine;

namespace Akasha.Data
{
    public class Sortie
    {
        public Aircraft Aircraft {  get; private set; }
        public DateTime SortieStartTime { get; private set; }
        public DateTime SortieEndTime { get; private set; }
        public int SortieIdx { get; private set; }

        public TimeSpan Elapsed { get; private set; }

        public EndReasons EndReason { get; private set; }

        private bool _isSortieEnded = false;

        public List<UnitInfo> KilledUnits { get; private set; } = new List<UnitInfo>();

        public float ExplodedNukes { get; private set; }

        public PlayerAircraftInfo selfInfo;

        public float JammingAmount { get; private set; } = 0f;

        public int DetectedTargets { get; private set; } = 0;

        public Sortie(Aircraft aircraft, int sortieSerialNumber) 
        {
            this.Aircraft = aircraft;
            this.SortieIdx = sortieSerialNumber;
            SortieStartTime = DateTime.UtcNow;
            SetSelfInfo();
        }

        public void AddJamming()
        {
            JammingAmount += Time.deltaTime;
        }

        public void AddDetectedTarget()
        {
            DetectedTargets++;
        }

        public void AddNuke()
        {
            ExplodedNukes++;
        }
        public override string ToString()
        {
            return base.ToString();
        }

        public void RegisterSortieEnd(EndReasons sortieEndReason)
        {
            if (!_isSortieEnded) 
            {
                EndReason = sortieEndReason;
                SortieEndTime = DateTime.UtcNow;
                _isSortieEnded = true;
                Elapsed = SortieEndTime - SortieStartTime;
            }
        }

        public void AddKill(PersistentUnit unit, string weaponName = null)
        {
            if (Aircraft.NetworkHQ == unit.unit.NetworkHQ) { return; }
            UnitInfo unitInfo = CopyUnitInfo(unit.unit);
            ulong id = 0;
            if (unit.unit is Aircraft a && a.Player != null)
                id = a.Player.SteamID;
            if (weaponName != null) unitInfo.SetDeathInfo(weaponName, unit.definition.unitName, id);
            KilledUnits.Add(unitInfo);
        }
      
        public void DetectKilled(PersistentID killerPersistentID, string weapon)
        {
            if (UnitRegistry.TryGetPersistentUnit(killerPersistentID, out var persistentUnit))
            {
                Unit unit = persistentUnit.unit;
                ulong id = 0;
                if (unit is Aircraft)
                {
                    Aircraft aircraft = (Aircraft)unit;
                    if (aircraft.Player != null)
                    {
                        id = aircraft.Player.SteamID;
                        RegisterSortieEnd(EndReasons.killedByPlayer);
                        return;
                    }
                }
                RegisterSortieEnd(EndReasons.killedByBot);

                selfInfo.SetDeathInfo(weapon, unit.definition.unitName, id);
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
            selfInfo = (PlayerAircraftInfo)CopyUnitInfo(Aircraft);    
        }
         
    }
}
