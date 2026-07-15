
namespace Akasha.Events
{
    public class UnitRegisteredEvent
    {
        public readonly Unit unit;
        public readonly PersistentID id;

        public UnitRegisteredEvent(Unit unit, PersistentID id)
        {
            this.unit = unit;
            this.id = id;
        }
    }
}
