using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akasha.Events;
using Akasha.Infrastructure;
using HarmonyLib;
using UnityEngine;

namespace Akasha
{
    [HarmonyPatch(typeof(UnitRegistry))]
    public class UnitPatch
    {
        [HarmonyPatch("RegisterUnit")]
        [HarmonyPostfix]
        public static void RegisterUnitPatch(Unit unit, PersistentID id)
        {
            var eventBus = ServiceLocator.Resolve<IEventBus>();
            eventBus.Publish<UnitRegisteredEvent>(new UnitRegisteredEvent(unit, id));
        }

        [HarmonyPatch("UnregisterUnit")]
        [HarmonyPostfix]
        public static void UnregisterUnitPatch(Unit unit)
        {
            var eventBus = ServiceLocator.Resolve<IEventBus>();
            eventBus.Publish<UnitUnregisteredEvent>(new UnitUnregisteredEvent(unit));
        }
    }
}
