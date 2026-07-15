using System.Collections.Generic;
using System.Linq;
using NuclearOption.Networking;
using UnityEngine;
using Akasha.Infrastructure;
using Akasha.Events;
namespace Akasha.WeaponLogging;

public static class WeaponLoggingExtensions
{
    public static void RecordDamage(
        this Unit unit,
        PersistentID lastDamagedBy,
        float damageAmount,
        string weaponName)
    {
        if (unit == null)
        {
            AkashaPlugin.Logger.LogError("Unit is null in recordDamage!");
            return;
        }
        unit.damageCredit ??= new Dictionary<PersistentID, float>();

        unit.damageCredit.TryGetValue(lastDamagedBy, out var originalDamageAmount);
        unit.damageCredit[lastDamagedBy] = originalDamageAmount + damageAmount;

        var state = AkashaPlugin.WeaponStorage.Get(unit);
        var weaponCredit = state.WeaponCredit;

        if (!weaponCredit.TryGetValue(lastDamagedBy, out var existingDamageCredit))
        {
            existingDamageCredit = new Dictionary<string, float>
            {
                [weaponName] = damageAmount
            };
        }
        else
        {
            existingDamageCredit[weaponName] =
                existingDamageCredit.TryGetValue(weaponName, out var current)
                    ? current + damageAmount
                    : damageAmount;
        }

        weaponCredit[lastDamagedBy] = existingDamageCredit;
    }

    public static void ReportKilled(this Unit unit)
    {
        var killerID = PersistentID.None;
        if (!UnitRegistry.TryGetPersistentUnit(unit.persistentID, out var killedUnit))
            return;

        var killedHQ = killedUnit.GetHQ();
        var killerDamage = 0.0f;
        var totalReceivedDamage = 0.0f;
        var state = AkashaPlugin.WeaponStorage.Get(unit);
        var weaponCredit = state.WeaponCredit;
        if (unit.damageCredit != null)

        {
            totalReceivedDamage += unit.damageCredit.Sum(keyValuePair => keyValuePair.Value);

            var damageDealerPlayers = new Dictionary<Player, float>();
            foreach (var receivedDamage in unit.damageCredit)
            {
                if (!UnitRegistry.TryGetPersistentUnit(receivedDamage.Key, out var damageDealerUnit)) continue;
                var dealtDamageProportion = receivedDamage.Value / totalReceivedDamage;
                if (dealtDamageProportion < 0.009999999776482582)
                    continue; 
                if (receivedDamage.Value >= (double)killerDamage)
                {
                    killerDamage = receivedDamage.Value;
                    killerID = receivedDamage.Key;
                }

                var damageDealerHQ = damageDealerUnit.GetHQ();
                if (killedHQ == damageDealerHQ || killedHQ == null) continue;
                var score = Mathf.Sqrt(killedUnit.definition.value) * dealtDamageProportion;
                var reward = score * damageDealerHQ.killReward;
                damageDealerHQ.AddScore(score);
                damageDealerHQ.AddFunds(reward * damageDealerHQ.playerTaxRate);
                if (damageDealerUnit.player == null) continue;

                if (!damageDealerPlayers.ContainsKey(damageDealerUnit.player))
                    damageDealerPlayers.Add(damageDealerUnit.player, 0.0f);
                damageDealerPlayers[damageDealerUnit.player] += dealtDamageProportion;
            }

            foreach (var damageDealer in damageDealerPlayers)
                damageDealer.Key.HQ.ReportKillAction(damageDealer.Key, unit, damageDealer.Value);
        }

        ulong? killedSteamID;
        Aircraft? killedAircraft;
        if (killedUnit.unit is Aircraft killedAircraftUnit)
        {
            killedAircraft = killedAircraftUnit;
            killedSteamID = killedAircraft.Player?.SteamID;
        }
        else
        {
            killedAircraft = null;
            killedSteamID = killedUnit.player?.SteamID;
        }
        ulong? killerSteamID;
        Aircraft? killerAircraft;

        var killerIsUnit = UnitRegistry.TryGetUnit(killerID, out var killerUnit);
        UnitRegistry.TryGetPersistentUnit(killerID, out var killerPUnit);
        if (killerUnit is Aircraft killerAircraftUnit)
        {
            killerAircraft = killerAircraftUnit;
            killerSteamID = killerAircraft.Player?.SteamID;
        }
        else
        {
            killerAircraft = null;
            killerSteamID = killerPUnit?.player?.SteamID;
        }

        KeyValuePair<string, float>? killerWeapon;
        string killerWeaponName;
        if (!weaponCredit.TryGetValue(killerID, out var killerAircraftWeapons))
        {
            killerWeaponName = "";
        }
        else if (killerAircraftWeapons is null)
        {
            AkashaPlugin.Logger.LogError(
                "This should not happen. Something killed something else without recording any dealt damage");
            killerWeaponName = "";
        }
        else
        {
            killerWeapon = killerAircraftWeapons.FirstOrDefault();
            foreach (var kvp in killerAircraftWeapons.Where(kvp => kvp.Value > killerWeapon.Value.Value))
            {
                killerWeapon = kvp;
            }
            killerWeaponName = killerWeapon?.Key ?? "";
        }

        string killedName;
        if (killedSteamID != null && killedUnit.unitName.IndexOf('[') != -1)
        {
            var s = killedUnit.unitName;
            killedName = s.Substring(s.IndexOf('[') + 1, s.IndexOf(']') - (s.IndexOf('[') + 1));
        }
        else killedName = killedUnit.unitName;
        
        string? killerName;
        if (killerSteamID != null && killerPUnit?.unitName.IndexOf('[') != -1)
        {
            var s = killerPUnit?.unitName;
            killerName = s?.Substring(s.IndexOf('[') + 1, s.IndexOf(']') - (s.IndexOf('[') + 1));
        }
        else killerName = killerPUnit?.unitName;

        AkashaPlugin.Logger.LogWarning($"{killedName} was killed by {killerName} with weapon {killerWeaponName}");
        

        var eventBus = ServiceLocator.Resolve<IEventBus>();
        eventBus.Publish<KillEvent>(new KillEvent(killerID, unit.persistentID, killerWeaponName));
        
        var killedType = unit switch
        {
            Missile _ => KillType.Missile,
            Building _ => KillType.Building,
            Aircraft _ => KillType.Aircraft,
            Ship _ => KillType.Ship,
            _ => KillType.Vehicle
        };
        
        if (NetworkSceneSingleton<MessageManager>.i == null)
            return;
        NetworkSceneSingleton<MessageManager>.i.RpcKillMessage(killerID, unit.persistentID, killedType);
    }
}
