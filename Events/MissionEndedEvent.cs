using NuclearOption.SavedMission.ObjectiveV2.Outcomes;

namespace Akasha.Events
{
    public class MissionEndedEvent
    {
        public readonly FactionHQ declarant;
        public readonly EndType endType;
        public MissionEndedEvent(FactionHQ declarant, EndType endType)
        {
            this.declarant = declarant;
            this.endType = endType;
        }
    }
}
