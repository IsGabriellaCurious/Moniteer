using MoniteerLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoniteerClient
{
    public class ClientHandle
    {
        public static void Welcome(Packet _packet)
        {
            string _msg = _packet.ReadString();
            int _id = _packet.ReadInt();

            Console.WriteLine($"Message from server: {_msg}");
            Client.instance.internalId = _id;
            ClientSend.WelcomeReceived();
        }
    }
}
