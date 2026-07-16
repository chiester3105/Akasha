using System.Collections.Generic;
using ProtoBuf;

namespace Akasha.Contracts
{
    [ProtoContract]
    public class PlayerRecord
    {
        [ProtoMember(1)] public ulong PlayerId { get; set; }
        [ProtoMember(2)] public List<SortieRecord> Sorties { get; set; }
        [ProtoMember(3)] public string Faction { get; set; }
        [ProtoMember(4)] public float Score { get; set; }
    }
}
