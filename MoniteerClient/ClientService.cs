using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using MoniteerLib;

namespace MoniteerClient
{
    public class ClientService
    {

        public Client client;

        Thread mainThread;
        public bool mainThreadRunning;

        public ClientService()
        {
            client = new Client();
        }

        public void Start(bool _console)
        {
            mainThreadRunning = true;
            mainThread = new Thread(new ThreadStart(MainThread));
            mainThread.Start();

            client.ConnectToServer(_console);
        }

        public void Stop()
        {
            client.Disconnect();
            mainThread.Abort();
            mainThreadRunning = false;
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

    }
}
