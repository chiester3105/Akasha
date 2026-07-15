using Mirage;

namespace Akasha.Events
{
    public class PlayerDisconnectedEvent
    {
        public readonly INetworkPlayer player;
        public PlayerDisconnectedEvent(INetworkPlayer player) => this.player = player;
    }
}
