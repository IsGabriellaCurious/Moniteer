using MoniteerLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace MoniteerServer
{
    public class Client
    {
        public static int dataBufferSize = 4096;
        public int internalId;
        public string machineName;
        public bool console = false;
        public bool consoleAuth = false;
        public TCP tcp;
        public UDP udp;

        public Client(int _internalId)
        {
            internalId = _internalId;
            tcp = new TCP(internalId);
            udp = new UDP(internalId);
        }

        public class TCP
        {
            public TcpClient socket;

            private readonly int internalId;
            private NetworkStream stream;
            private Packet receivedData;
            private byte[] receiveBuffer;

            public TCP(int _internalId)
            {
                internalId = _internalId;
            }

            public void Connect(TcpClient _socket)
            {
                socket = _socket;
                socket.ReceiveBufferSize = dataBufferSize;
                socket.SendBufferSize = dataBufferSize;

                stream = socket.GetStream();

                receivedData = new Packet();
                receiveBuffer = new byte[dataBufferSize];

                stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
                PacketSender.Welcome(internalId, "Successfully connected.");
            }

            public void SendData(Packet _packet)
            {
                try
                {
                    if (socket == null)
                        return;

                    stream.BeginWrite(_packet.ToArray(), 0, _packet.Length(), null, null);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Packet ERROR: {e}");
                }
            }

            private void ReceiveCallback(IAsyncResult _result)
            {
                try
                {
                    int _byteLength = stream.EndRead(_result);
                    if (_byteLength <=0 )
                    {
                        ServerService.clients[internalId].Disconnect();
                        return;
                    }

                    byte[] _data = new byte[_byteLength];
                    Array.Copy(receiveBuffer, _data, _byteLength);

                    receivedData.Reset(HandleData(_data));
                    stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
                } 
                catch (Exception e)
                {
                    Console.WriteLine($"Client ERROR: {e}");
                    ServerService.clients[internalId].Disconnect();
                }
            }

            private bool HandleData(byte[] _data)
            {
                int _packetLength = 0;

                receivedData.SetBytes(_data);

                if (receivedData.UnreadLength() >= 4)
                {
                    _packetLength = receivedData.ReadInt();
                    if (_packetLength <= 0)
                        return true;
                }

                while (_packetLength > 0 && _packetLength <= receivedData.UnreadLength())
                {
                    byte[] _packetBytes = receivedData.ReadBytes(_packetLength);
                    ThreadManager.ExecuteOnMainThread(() =>
                    {
                        using (Packet _packet = new Packet(_packetBytes))
                        {
                            int _packetId = _packet.ReadInt();
                            ServerService.packetHandlers[_packetId](internalId, _packet);
                        }
                    });

                    _packetLength = 0;
                    if (receivedData.UnreadLength() >= 4)
                    {
                        _packetLength = receivedData.ReadInt();
                        if (_packetLength <= 0)
                            return true;
                    }
                }

                if (_packetLength <= 1)
                    return true;
                else
                    return false;
            }

            public void Disconnect()
            {
                socket.Close();
                stream = null;
                receivedData = null;
                receiveBuffer = null;
                stream = null;
            }
        }

        public class UDP
        {
            public IPEndPoint endPoint;

            private int internalId;

            public UDP(int _internalId)
            {
                internalId = _internalId;
            }

            public void Connect(IPEndPoint _endPoint)
            {
                endPoint = _endPoint;
            }

            public void SendData(Packet _packet)
            {
                ServerService.SendUDPData(endPoint, _packet);
            }

            public void HandleData(Packet _packetData)
            {
                int _packetLength = _packetData.ReadInt();
                byte[] _packetBytes = _packetData.ReadBytes(_packetLength);

                ThreadManager.ExecuteOnMainThread(() =>
                {
                    using (Packet _packet = new Packet(_packetBytes))
                    {
                        int _packetId = _packet.ReadInt();
                        ServerService.packetHandlers[_packetId](internalId, _packet);
                    }
                });
            }

            public void Disconnect()
            {
                endPoint = null;
            }
        }

        private void Disconnect()
        {
            Console.WriteLine($"DEBUG: {tcp.socket.Client.RemoteEndPoint} has disconnected.");

            ServerService.clients.Remove(internalId);
            ServerService.clientIds.Remove(internalId);

            tcp.Disconnect();
            udp.Disconnect();
        }
    }
}
