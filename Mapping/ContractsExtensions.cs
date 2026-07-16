using System.Collections.Generic;
using System.Linq;
using Akasha.Contracts;
using Akasha.Data;

namespace Akasha.Mapping
{
    public static class ContractsExtensions
    {
        public static void MapContract(this SortieRecord contract, Sortie sortie)
        {
            contract.AircraftName = sortie.Aircraft.definition.unitName;
            contract.EndReason = sortie.EndReason.ToString();
            contract.LiveTime = (sortie.SortieEndTime - sortie.SortieStartTime).TotalSeconds;
            contract.JammingAmount = sortie.JammingAmount;
            contract.DetectedTargets = sortie.DetectedTargets;

            contract.Kills = sortie.KilledUnits.ToKillRecords();
        }

        public static void MapContract(this KillRecord contract, UnitInfo unit)
        {
            contract.UsedWeapon = unit.killedByWeapon;
            contract.KilledUnit = unit.UnitName;

            if(unit is PlayerAircraftInfo player)
            {
                contract.KilledPlayerId = player.SteamID;
            }
        }

        public static void MapContract(this PlayerRecord contract, IEnumerable<Sortie> sorties, PlayerSavedData data)
        {
            contract.PlayerId = data.SteamID;
            contract.Faction = data.FactionName;
            contract.Score = data.Score;

            contract.Sorties = sorties.ToSortieRecords();
        }

        public static List<KillRecord> ToKillRecords(this IEnumerable<UnitInfo> units)
        {
            return units.Select(unit =>
            {
                var record = new KillRecord();
                record.MapContract(unit);
                return record;
            }
            ).ToList();
        }

        public static List<SortieRecord> ToSortieRecords(this IEnumerable<Sortie> sorties)
        {
            return sorties.Select(sortie => 
            {
                var record = new SortieRecord();
                record.MapContract(sortie);
                return record;
            }).ToList();
        }
       // public static void MapContract(this Match)
    }
}
