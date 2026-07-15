using Akasha.Events;
using Akasha.Infrastructure;
using HarmonyLib;
using NuclearOption.SavedMission;

namespace Akasha
{
    [HarmonyPatch(typeof(Mission))]
    public static class MissionPatch
    {
        [HarmonyPatch("AfterLoad", typeof(MissionKey))]
        [HarmonyPrefix] 
        public static void Prefix(Mission __instance, MissionKey key)
        {
            var eventBus = ServiceLocator.Resolve<IEventBus>();
            eventBus.Publish<MissionLoadedEvent>(new MissionLoadedEvent(__instance.Name));
        }
    }
}
