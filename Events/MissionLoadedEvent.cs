namespace Akasha.Events
{
    public class MissionLoadedEvent
    {
        public readonly string mapName;
        public readonly string missionName;
        public MissionLoadedEvent(string path, string missionName)
        {
            this.missionName = missionName;

            if (path == "Terrain1") mapName = "Heartland";
            else if (path == "Terrain_naval") mapName = "Ignus";
            else mapName = path;
        }
    }
}
