using MoniteerLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoniteerServer
{
    class PacketSender
    {
        private static void SendTCPData(int _clientId, Packet _packet)
        {
            _packet.WriteLength();
            ServerService.clients[_clientId].tcp.SendData(_packet);
        }
        public static void Welcome(int _clientId, string _msg)
        {
            using (Packet _packet = new Packet((int)ServerPackets.welcome))
            {
                _packet.Write(_msg);
                _packet.Write(_clientId);

                SendTCPData(_clientId, _packet);
            }
        }
    }
}
