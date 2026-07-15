using System.Collections.Generic;

namespace Akasha.Data
{
    public class UnitInfo
    {
        public string UnitName { get; private set; }
        public PersistentID persistentID { get; private set; }

        public List<WeaponInfo> weaponsInfo = new List<WeaponInfo>();
        public string killedByWeapon { get; private set; }
        public bool IsParachuted { get; private set; }
        public virtual void CopyUnitInfo(Unit unit)
        {
            UnitName = unit.unitName;
            persistentID = unit.persistentID;

            foreach (WeaponStation station in unit.weaponStations)
            {
                WeaponInfo weaponInfo = new WeaponInfo();
                weaponInfo.count = station.Ammo;
                weaponInfo.name = station.WeaponInfo.weaponName;
                weaponInfo.jammer = station.WeaponInfo.jammer;
                weaponsInfo.Add(weaponInfo);
            }
        }
        public void SetKillWeapon(string weaponName)
        {
            killedByWeapon = weaponName;
        }
        public override string ToString()
        {
            return $"UnitName: {UnitName}";
        }
    }
}
