using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using MoniteerLib;
using System.Threading;

namespace MoniteerServer
{
    public class ServerService
    {

        private TcpListener tcpListener;
        private static UdpClient udpListener;
        public static Dictionary<int, Client> clients = new Dictionary<int, Client>();
        public delegate void PacketHandler(int _client, Packet _packet);
        public static Dictionary<int, PacketHandler> packetHandlers;

        Thread mainThread;
        public bool mainThreadRunning; 

        private int currentId = 0; //top quality id method
        public static string password = "mrjames"; //quality password shit

        public ServerService()
        {
            mainThreadRunning = false;
        }

        public void Start()
        {
            tcpListener = new TcpListener(IPAddress.Any, 971);
            tcpListener.Start();
            tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);

            udpListener = new UdpClient(971);
            udpListener.BeginReceive(UDPReceiveCallback, null);

            InitializeServerData();
            mainThreadRunning = true;
            mainThread = new Thread(new ThreadStart(MainThread));
            mainThread.Start();

            Console.WriteLine("Moniteer Server has started!");
        }

        public void Stop()
        {
            tcpListener.Stop();
            mainThread.Abort();
            mainThreadRunning = false;
            Console.WriteLine("Moniteer Server has stopped.");
        }

        private void TCPConnectCallback(IAsyncResult _result)
        {
            TcpClient _client = tcpListener.EndAcceptTcpClient(_result);
            if (_client == null)
                return;
            tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);

            clients.Add(currentId, new Client(currentId));
            clients[currentId].tcp.Connect(_client);

            Console.WriteLine($"Client connected: {_client.Client.RemoteEndPoint} with ID {currentId}");
            currentId++;
        }

        private void UDPReceiveCallback(IAsyncResult _result)
        {
            try
            {
                IPEndPoint _clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
                byte[] _data = udpListener.EndReceive(_result, ref _clientEndPoint);
                udpListener.BeginReceive(UDPReceiveCallback, null);

                if (_data.Length < 4)
                    return;

                using (Packet _packet = new Packet(_data))
                {
                    int _clientId = _packet.ReadInt();

                    if (clients[_clientId] == null)
                    {
                        Console.WriteLine($"UDP Packet ERROR: Received data from client {_clientId} which does not exist!");
                        return;
                    }

                    if (clients[_clientId].udp.endPoint == null) //new connection
                    {
                        clients[_clientId].udp.Connect(_clientEndPoint);
                        return;
                    }

                    if (clients[_clientId].udp.endPoint.ToString() != _clientEndPoint.ToString())
                    {
                        Console.WriteLine($"UDP Packet ERROR: {_clientEndPoint.ToString()} tried to impersonate client {_clientId}. Packet discarded.");
                        return;
                    }

                    clients[_clientId].udp.HandleData(_packet);
                }
            } 
            catch (Exception e)
            {
                Console.WriteLine($"UDP Packet ERROR: {e}");
            }
        }

        public static void SendUDPData(IPEndPoint _clientEndPoint, Packet _packet)
        {
            try
            {
                if (_clientEndPoint == null)
                    return;

                udpListener.BeginSend(_packet.ToArray(), _packet.Length(), _clientEndPoint, null, null);
            }
            catch (Exception e)
            {
                Console.WriteLine($"UDP Packet ERROR: Error sending data to {_clientEndPoint}: {e}");
            }
        }

        private void MainThread()
        {
            Console.WriteLine($"Started the main thread running at {Constants.TICKS_PER_SECOND} ticks per second.");
            DateTime _nextLoop = DateTime.Now;

            while (mainThreadRunning)
            {
                while (_nextLoop < DateTime.Now)
                {
                    ThreadManager.UpdateMain();

                    _nextLoop = _nextLoop.AddMilliseconds(Constants.MS_PER_TICK);

                    if (_nextLoop > DateTime.Now)
                    {
                        Thread.Sleep(_nextLoop - DateTime.Now);
                    }
                }
            }
        }

        private void InitializeServerData()
        {
            packetHandlers = new Dictionary<int, PacketHandler>()
            {
                { (int)ClientPackets.welcomeReceived, ServerHandle.WelcomeReceived },
                { (int)ClientPackets.passwordCheck, ServerHandle.PasswordCheck }
            };
        }

    }
}
