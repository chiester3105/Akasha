namespace Akasha.Events
{
    public class UnitUnregisteredEvent
    {
        public readonly Unit unit;

        public UnitUnregisteredEvent(Unit unit) => this.unit = unit;
    }
}
