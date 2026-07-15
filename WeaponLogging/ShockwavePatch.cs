using HarmonyLib;

namespace Akasha.WeaponLogging;

[HarmonyPatch(typeof(Shockwave))]
public class ShockwavePatch
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(Shockwave.Update))]
    public static bool UpdatePrefix(Shockwave __instance)
    {
        MissileExtensions.Update(__instance);
        return false;
    }
}
