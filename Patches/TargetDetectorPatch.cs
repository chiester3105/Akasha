using Akasha.Events;
using Akasha.Infrastructure;
using HarmonyLib;

namespace Akasha.Patches
{
    [HarmonyPatch(typeof(TargetDetector))]
    public class TargetDetectorPatch
    {
        [HarmonyPatch("DetectTarget")]
        [HarmonyPrefix]
        public static void DetectTargetPrefix(TargetDetector __instance, Unit target)
        {     
            var attachedUnit = __instance.attachedUnit;
            if (attachedUnit is Aircraft detector)
            {
                if (detector.Player != null)
                {
                    var eventBus = ServiceLocator.Resolve<IEventBus>();
                    if (!detector.NetworkHQ.trackingDatabase.ContainsKey(target.persistentID))
                    {
                        eventBus.Publish<TargetDetectedEvent>(new TargetDetectedEvent(detector.persistentID));
                    }
                    else if (!detector.NetworkHQ.IsTargetPositionAccurate(target, 500))
                    {
                        eventBus.Publish<TargetDetectedEvent>(new TargetDetectedEvent(detector.persistentID));
                    }
                }
            }
        }
    }
}
