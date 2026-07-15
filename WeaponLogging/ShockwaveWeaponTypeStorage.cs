using System.Runtime.CompilerServices;

namespace Akasha.WeaponLogging;

public class ShockwaveWeaponTypeLog
{
    public string WeaponName = "";
}

public class ShockwaveWeaponTypeStorage
{
    private readonly ConditionalWeakTable<Shockwave, ShockwaveWeaponTypeLog> _table = new();

    public ShockwaveWeaponTypeLog Get(Shockwave shockwave)
    {
        return _table.GetOrCreateValue(shockwave);
    }
}