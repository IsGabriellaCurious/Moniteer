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
        public static Dictionary<int, Client> clients = new Dictionary<int, Client>();
        public delegate void PacketHandler(int _client, Packet _packet);
        public static Dictionary<int, PacketHandler> packetHandlers;

        Thread mainThread;
        public bool mainThreadRunning; 

        private int currentId = 0; //top quality id method

        public ServerService()
        {
            mainThreadRunning = false;
        }

        public void Start()
        {
            tcpListener = new TcpListener(IPAddress.Any, 971);
            tcpListener.Start();
            tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);

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
            tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);

            clients.Add(currentId, new Client(currentId));
            clients[currentId].tcp.Connect(_client);

            Console.WriteLine($"Client connected: {_client.Client.RemoteEndPoint} with ID {currentId}");
            currentId++;
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
                { (int)ClientPackets.welcomeReceived, ServerHandle.WelcomeReceived }
            };
        }

    }
}
