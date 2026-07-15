using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akasha.Events;
using Akasha.Infrastructure;
using HarmonyLib;
using Mirage;
using NuclearOption.Networking;
namespace Akasha.Patches
{
    // I dont understand why it named as ServerConnected/ServerDisconnected
    // if it is related to clients
    [HarmonyPatch(typeof(NetworkManagerNuclearOption))]
    public class NetworkManagerNuclearOptionPatch
    {
        [HarmonyPatch(nameof(NetworkManagerNuclearOption.OnServerAuthenticated))]
        public static void OnServerConnected(INetworkPlayer player)
        {
            var eventBus = ServiceLocator.Resolve<IEventBus>();
            eventBus.Publish<PlayerConnectedEvent>(new PlayerConnectedEvent(player));
        }

        [HarmonyPatch(nameof(NetworkManagerNuclearOption.OnServerDisconnect))]
        public static void OnServerDisconnected(INetworkPlayer player)
        {
            var eventBus = ServiceLocator.Resolve<IEventBus>();
            eventBus.Publish<PlayerDisconnectedEvent>(new PlayerDisconnectedEvent(player));
        }
    }
}
