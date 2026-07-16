using ProtoBuf;

namespace Akasha.Contracts
{
    [ProtoContract]
    public class KillRecord
    {
        [ProtoMember(1)] public string KilledUnit { get; set; }
        [ProtoMember(2)] public ulong? KilledPlayerId { get; set; }
        [ProtoMember(3)] public string UsedWeapon {  get; set; }
    }
}
