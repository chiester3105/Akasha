using HarmonyLib;


namespace Akasha.WeaponLogging;


[HarmonyPatch(typeof(Unit))]
public class UnitPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(Unit.ReportKilled))]
    public static bool ReportKilledPrefix(Unit __instance)
    {
        WeaponLoggingExtensions.ReportKilled(__instance);
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(Unit.RecordDamage))]
    public static bool RecordDamagePrefix(Unit __instance, PersistentID lastDamagedBy, float damageAmount)
    {
        AkashaPlugin.Logger.LogError("Original Unit.RecordDamage was called.");
        return true;
    }
}
