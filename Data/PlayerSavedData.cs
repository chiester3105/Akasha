using System;

namespace Akasha.Data
{
    public class PlayerSavedData
    {
        public ulong SteamID { get; private set; }
        public string Name { get; private set; }
        public float Score { get; private set; }
        public FactionHQ FactionHQ { get; private set; }
        public string FactionName { get; private set; }
        public PlayerSavedData(ulong steamID, string name, float score, FactionHQ factionHQ)
        {
            try
            {
                SteamID = steamID;
                Name = name;
                Score = score;
                FactionHQ = factionHQ;
                FactionName = factionHQ.faction.factionName;
            }
            catch (Exception ex)
            {
                AkashaPlugin.DebugLog($"Exception in PlayerSavedData .ctor: {ex.Message}");
            }
        }
    }


}
