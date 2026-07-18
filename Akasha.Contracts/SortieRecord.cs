using System.Collections.Generic;
using ProtoBuf;

namespace Akasha.Contracts
{
    [ProtoContract]
    public class SortieRecord
    {
        [ProtoMember(1)] public string AircraftName { get; set; }
        [ProtoMember(2)] public List<KillRecord> Kills { get; set; }
        [ProtoMember(3)] public string EndReason { get; set; }
        [ProtoMember(4)] public long StartTime { get; set; } //unix
        [ProtoMember(5)] public long EndTime { get; set; } //unix 
        [ProtoMember(6)] public float JammingAmount { get; set; } // jamming pod only (NOT ECM!)
        [ProtoMember(7)] public int DetectedTargets { get; set; }
        [ProtoMember(8)] public string KilledByWeapon { get; set; }
        [ProtoMember(9)] public string KilledByUnit { get; set; }
        [ProtoMember(10)] public ulong? KilledByPlayer {  get; set; }
        [ProtoMember(11)] public int SortieIdx { get; set; } //local idx, use composite primary key (matchId, sortieIdx) in database
    }
}
