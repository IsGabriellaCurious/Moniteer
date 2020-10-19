using MoniteerLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoniteerClient
{
    public class ClientSend
    {
        private static void SendTCPData(Packet _packet)
        {
            _packet.WriteLength();
            Client.instance.tcp.SendData(_packet);
        }

        private static void SendUDPData(Packet _packet)
        {
            _packet.WriteLength();
            Client.instance.udp.SendData(_packet);
        }

        public static void WelcomeReceived()
        {
            using (Packet _packet = new Packet((int)ClientPackets.welcomeReceived))
            {
                _packet.Write(Client.instance.internalId);
                _packet.Write(Environment.MachineName.ToString());
                _packet.Write(Client.instance.console);

                SendTCPData(_packet);
            }
        }

        public static void PasswordCheck(String _password)
        {
            using (Packet _packet = new Packet((int)ClientPackets.passwordCheck))
            {
                _packet.Write(Client.instance.internalId);
                _packet.Write(_password);

                SendTCPData(_packet);
            }
        }

        public static void ClientList()
        {
            using (Packet _packet = new Packet((int)ClientPackets.clientList))
            {
                _packet.Write(Client.instance.internalId);

                SendTCPData(_packet);
            }
        }
    }
}
