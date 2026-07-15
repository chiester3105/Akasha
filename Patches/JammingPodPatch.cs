using Akasha.Events;
using Akasha.Infrastructure;
using HarmonyLib;
using UnityEngine;
namespace Akasha.Patches
{
    [HarmonyPatch(typeof(JammingPod), "Fire")]
    public class JammingPodPatch
    {
        [HarmonyPostfix]
        public static void Fire(JammingPod __instance, Unit owner, Unit target, Vector3 inheritedVelocity, WeaponStation weaponStation, GlobalPosition aimpoint)
        {
            var eventBus = ServiceLocator.Resolve<IEventBus>();
            eventBus.Publish<JammingEvent>(new JammingEvent(owner.persistentID));
        }
    }
}
