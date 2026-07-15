using System.Collections.Generic;
using System.Runtime.CompilerServices;


namespace Akasha.WeaponLogging;

public class UnitWeaponLogState
{
    public readonly Dictionary<PersistentID, Dictionary<string, float>> WeaponCredit = new();
}

public class UnitWeaponLogStorage
{
    private readonly ConditionalWeakTable<Unit, UnitWeaponLogState> _table = new();

    public UnitWeaponLogState Get(Unit unit)
    {
        return _table.GetOrCreateValue(unit);
    }
}
