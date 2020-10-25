using MoniteerLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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

            //Client.instance.udp.Connect(((IPEndPoint)Client.instance.tcp.socket.Client.LocalEndPoint).Port);
        }

        public static void PasswordCheckResponse(Packet _packet)
        {
            Client.instance.consoleHandlers[(int)ServerPackets.passwordCheckResponse](_packet);
        }

        public static void ClientListResponse(Packet _packet)
        {
            Client.instance.consoleHandlers[(int)ServerPackets.clientListResponse](_packet);
        }

        public static void ConsoleMsgForward(Packet _packet)
        {
            string _msg = _packet.ReadString();
            string _from = _packet.ReadString();

            MessageBox.Show(_msg, "Message from " + _from);
        }

    }
}
