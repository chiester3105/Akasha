using Mirage;

namespace Akasha.Events
{
    public class PlayerConnectedEvent
    {
        public readonly INetworkPlayer player;
        public PlayerConnectedEvent(INetworkPlayer player) => this.player = player;
    }
}
