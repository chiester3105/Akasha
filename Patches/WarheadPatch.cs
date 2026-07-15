using Akasha.Events;
using Akasha.Infrastructure;
using HarmonyLib;
using UnityEngine;

namespace Akasha.Patches
{
    [HarmonyPatch(typeof(Missile.Warhead), "Detonate")]
    public class WarheadPatch
    {
        public static void Prefix(Missile.Warhead __instance, Rigidbody rb, PersistentID ownerID, Vector3 position, Vector3 normal, bool armed, float blastYield, bool hitArmor, bool hitTerrain)
        {
            if (blastYield >= 1500000)
            {
                PersistentUnit unit;
                if (UnitRegistry.TryGetPersistentUnit(ownerID, out unit))
                {
                    float blastRadius = Mathf.Pow((blastYield / 1000000) * 1000000f, 0.3333f) * 13;
                    if (unit.player != null)
                    {
                        var bus = ServiceLocator.Resolve<IEventBus>();
                        bus.Publish<NuclearWarheadDetonatedEvent>(new NuclearWarheadDetonatedEvent(ownerID));
                    }
                }               
            }
        }
    }
}
