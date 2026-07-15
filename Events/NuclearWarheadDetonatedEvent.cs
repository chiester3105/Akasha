namespace Akasha.Events
{
    public class NuclearWarheadDetonatedEvent
    {
        public readonly PersistentID id;
        public NuclearWarheadDetonatedEvent(PersistentID id) => this.id = id;
    }
}
