namespace Akasha.Events
{
    public class MissionLoadedEvent
    {
        public readonly string mapName;
        public MissionLoadedEvent(string mapName) => this.mapName = mapName;
    }
}
