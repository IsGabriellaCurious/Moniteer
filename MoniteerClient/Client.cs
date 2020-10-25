using MoniteerLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MoniteerClient
{
    public class Client
    {
        public static Client instance;
        public static int dataBufferSize = 4096;

        public string ip = "127.0.0.1";
        public int port = 971;
        public int internalId;
        public TCP tcp;
        public UDP udp;

        private bool isConnected = false;
        public bool console;

        private delegate void PacketHandler(Packet _packet);
        private static Dictionary<int, PacketHandler> packetHandlers;

        public Dictionary<int, Constants.ConsoleHandler> consoleHandlers;

        public Client()
        {
            if (instance == null)
            {
                instance = this;
            }
            tcp = new TCP();
            udp = new UDP();
        }

        public void ConnectToServer(bool _console)
        {
            InitializeClientData();
            isConnected = true;
            console = _console;
            tcp.Connect();
        }

        public class TCP
        {
            public TcpClient socket;

            private NetworkStream stream;
            private Packet receivedData;
            private byte[] receiveBuffer;

            public void Connect()
            {
                socket = new TcpClient
                {
                    ReceiveBufferSize = dataBufferSize,
                    SendBufferSize = dataBufferSize
                };

                receiveBuffer = new byte[dataBufferSize];
                socket.BeginConnect(instance.ip, instance.port, ConnectCallback, socket);
            }

            public void ConnectCallback(IAsyncResult _result)
            {
                try
                {
                    socket.EndConnect(_result);

                    if (!socket.Connected)
                        return;

                    stream = socket.GetStream();

                    receivedData = new Packet();

                    stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
                } 
                catch (Exception e)
                {
                    Console.WriteLine("Server refused the connection. Is it online?");
                    Disconnect();
                }
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
                    Console.WriteLine($"TCP Packet ERROR: {e}");
                }
            }

            private void ReceiveCallback(IAsyncResult _result)
            {
                try
                {
                    int _byteLength = stream.EndRead(_result);
                    if (_byteLength <= 0)
                    {
                        instance.Disconnect();
                        return;
                    }

                    byte[] _data = new byte[_byteLength];
                    Array.Copy(receiveBuffer, _data, _byteLength);

                    receivedData.Reset(HandleData(_data));

                    stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"TCP Client ERROR: {e}");
                    Disconnect();
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
                            packetHandlers[_packetId](_packet);
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

            private void Disconnect()
            {
                instance.Disconnect();

                stream = null;
                receivedData = null;
                receiveBuffer = null;
                socket = null;
            }
        }

        public class UDP
        {
            public UdpClient socket;
            public IPEndPoint endPoint;

            public UDP()
            {
                endPoint = new IPEndPoint(IPAddress.Parse(instance.ip), instance.port);
            }

            public void Connect(int _localPort)
            {
                socket = new UdpClient(_localPort);
                socket.Connect(endPoint);
                socket.BeginReceive(ReceiveCallback, null);

                using (Packet _packet = new Packet())
                {
                    SendData(_packet);
                }
            }

            public void SendData(Packet _packet)
            {
                try
                {
                    _packet.InsertInt(instance.internalId);

                    if (socket == null)
                        return;

                    socket.BeginSend(_packet.ToArray(), _packet.Length(), null, null);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"UDP Packet ERROR: {e}");
                }
            }
        
            private void ReceiveCallback(IAsyncResult _result)
            {
                try
                {
                    byte[] _data = socket.EndReceive(_result, ref endPoint);
                    socket.BeginReceive(ReceiveCallback, null);

                    if (_data.Length < 4)
                    {
                        instance.Disconnect();
                        return;
                    }

                    HandleData(_data);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"UDP Client ERROR: {e}");
                    Disconnect();
                }
            }

            private void HandleData(byte[] _data)
            {
                using (Packet _packet = new Packet(_data))
                {
                    int _packetLength = _packet.ReadInt();
                    _data = _packet.ReadBytes(_packetLength);
                }

                ThreadManager.ExecuteOnMainThread(() =>
                {
                    using (Packet _packet = new Packet(_data))
                    {
                        int _packetId = _packet.ReadInt();
                        packetHandlers[_packetId](_packet);
                    }
                });
            }

            private void Disconnect()
            {
                instance.Disconnect();

                endPoint = null;
                socket = null;
            }
        
        }

        private void InitializeClientData()
        {
            packetHandlers = new Dictionary<int, PacketHandler>()
            {
                { (int)ServerPackets.welcome, ClientHandle.Welcome },
                { (int)ServerPackets.passwordCheckResponse, ClientHandle.PasswordCheckResponse },
                { (int)ServerPackets.clientListResponse, ClientHandle.ClientListResponse },
                { (int)ServerPackets.consoleMsgForward, ClientHandle.ConsoleMsgForward }
            };
            Console.WriteLine("Initlaized packets");
        }

        public void Disconnect()
        {
            if (isConnected)
            {
                isConnected = false;
                tcp.socket.Close();
                //udp.socket.Close();

                Console.WriteLine("Disconnected from server.");
            }
        }

    }
}
