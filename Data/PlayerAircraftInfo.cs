using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Akasha.Data
{
    public class PlayerAircraftInfo : UnitInfo
    {
        public ulong SteamID { get; private set; }
        public string PlayerName { get; private set; }
        public string AircraftName { get; private set; }
        public void CopyPlayerInfo(Aircraft aircraft)
        {
            CopyUnitInfo(aircraft);
            SteamID = aircraft.Player.GetAuthData().SteamID.m_SteamID;
            PlayerName = aircraft.Player.PlayerName;
            AircraftName = aircraft.definition.unitName;
        }
        public override string ToString()
        {
            if (PlayerName != null)
            {
                return base.ToString() + $", SteamID: {SteamID}";
            }
            else return base.ToString();
        }
    }
}
