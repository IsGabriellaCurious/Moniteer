using Microsoft.SqlServer.Server;
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
            bool console = _packet.ReadBool();

            if (!IdAuthentic(_from, _id))
                return;

            if (console)
                ServerService.clients[_id].console = true;

            ServerService.clients[_id].machineName = machineName;
            ServerService.clientIds.Add(_id, machineName);

            Console.WriteLine($"DEBUG: Client ID {_id} WELCOME RESPONSE: NAME {machineName}, IP {ServerService.clients[_from].tcp.socket.Client.RemoteEndPoint}, CONSOLE: {console.ToString()}");
        }

        public static void PasswordCheck(int _from, Packet _packet)
        {
            int _id = _packet.ReadInt();
            string password = _packet.ReadString();

            if (!IdAuthentic(_from, _id))
                return;

            PacketSender.PasswordVaild(_id, password.Equals(ServerService.password));
        }

        public static void ClientList(int _from, Packet _packet)
        {
            int _id = _packet.ReadInt();

            if (!IdAuthentic(_from, _id) || !IsConsole(_id) || !ConsoleAuthed(_id))
                return;

            PacketSender.ClientList(_id);
        }


        public static void ConsoleMsgSend(int _from, Packet _packet)
        {
            int _id = _packet.ReadInt();

            if (!IdAuthentic(_from, _id) || !IsConsole(_id) || !ConsoleAuthed(_id))
                return;

            int _to = _packet.ReadInt();
            string _msg = _packet.ReadString();
            string _fromUser = _packet.ReadString();

            PacketSender.ConsoleMsgForward(_to, _msg, _fromUser);
        }

        private static bool IdAuthentic(int _from, int _received)
        {
            if (_from != _received)
            {
                Console.WriteLine($"!! FATAL ERROR !! Machine with Server ID {_from} has returned an invalid ID: {_received}");
                return false;
            }
            return true;
        }

        private static bool IsConsole(int _id)
        {
            return ServerService.clients[_id].console;
        }

        private static bool ConsoleAuthed(int _id)
        {
            return ServerService.clients[_id].consoleAuth;
        }
    }
}
