using MoniteerLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoniteerServer
{
    public class ServerHandle
    {
        public static void WelcomeReceived(int _from, Packet _packet)
        {
            int _id = _packet.ReadInt();
            string machineName = _packet.ReadString();

            if (_id != _from)
            {
                Console.WriteLine($"!! FATAL ERROR !! Machine name {machineName} with Server ID {_from} has returned an invalid ID: {_id}");
                return;
            }
            Console.WriteLine($"DEBUG: Client ID {_id} WELCOME RESPONSE: NAME {machineName}, IP {ServerService.clients[_from].tcp.socket.Client.RemoteEndPoint}");
        }
    }
}
