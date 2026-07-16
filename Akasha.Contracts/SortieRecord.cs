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
        [ProtoMember(4)] public double LiveTime { get; set; }
        [ProtoMember(5)] public float JammingAmount { get; set; } // jamming pod only (NOT ECM!)
        [ProtoMember(6)] public int DetectedTargets { get; set; }

    }
}
