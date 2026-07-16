using System.Collections.Generic;
using ProtoBuf;

namespace Akasha.Contracts
{
    [ProtoContract]
    public class MatchRecord
    {
        [ProtoMember(1)] public string Winner { get; set; }
        [ProtoMember(2)] public float Duration { get; set; }
        [ProtoMember(3)] public List<PlayerRecord> Players { get; set; }
        [ProtoMember(4)] public long StartTimeUnix { get; set; }
        [ProtoMember(5)] public string ServerId { get; set; }
        [ProtoMember(6)] public string MapName { get; set; } //unity scene/world, not mission name
        [ProtoMember(7)] public string MissionName { get; set; }
    }
}
