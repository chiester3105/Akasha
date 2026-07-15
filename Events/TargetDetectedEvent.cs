namespace Akasha.Events
{
    public class TargetDetectedEvent
    {
        public readonly PersistentID detector;
        public TargetDetectedEvent(PersistentID detector) => this.detector = detector;
    }
}
