using System.Collections.Generic;

namespace Akasha.Data
{
    public class UnitInfo
    {
        public string UnitName { get; private set; }
        public PersistentID PersistentID { get; private set; }

        public List<WeaponInfo> weaponsInfo = new List<WeaponInfo>();
        public string KilledByWeapon { get; private set; }
        public string KilledByUnit { get; private set; }
        public ulong? KilledByPlayer { get; private set; }
        public virtual void CopyUnitInfo(Unit unit)
        {
            UnitName = unit.definition.unitName;
            PersistentID = unit.persistentID;

            foreach (WeaponStation station in unit.weaponStations)
            {
                WeaponInfo weaponInfo = new WeaponInfo();
                weaponInfo.count = station.Ammo;
                weaponInfo.name = station.WeaponInfo.weaponName;
                weaponInfo.jammer = station.WeaponInfo.jammer;
                weaponsInfo.Add(weaponInfo);
            }
        }
        public void SetDeathInfo(string weaponName, string unit, ulong playerId)
        {
            KilledByWeapon = weaponName;
            KilledByUnit = unit;
            KilledByPlayer = playerId;
        }
        public override string ToString()
        {
            return $"UnitName: {UnitName}";
        }
    }
}
