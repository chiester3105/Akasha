using Akasha.Events;
using Akasha.Infrastructure;
using HarmonyLib;
using NuclearOption.SavedMission.ObjectiveV2.Outcomes;

namespace Akasha
{
    [HarmonyPatch(typeof(FactionHQ))]
    public class FactionHQPatch
    {       
        [HarmonyPatch("DeclareEndGame")]
        [HarmonyPrefix]
        public static void DeclareEndGamePatch(FactionHQ __instance, EndType endType)
        {
            var eventBus = ServiceLocator.Resolve<IEventBus>();
            eventBus.Publish<MissionEndedEvent>(new MissionEndedEvent(__instance, endType));
        }
    }
}
