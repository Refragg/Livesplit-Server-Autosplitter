using System;
using System.Net.Sockets;
using System.Threading;
using System.Timers;
using Timer = System.Timers.Timer;

namespace Livesplit_Server_Autosplitter
{
    class Program
    {
        public static Socket socket;
        public static AutosplitterRuntime Runtime;

        public static Timer refreshTimer = new Timer();
        
        static void Main(string[] args)
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Unspecified);
            socket.Connect("localhost", 16834);
            
            //Arbitrary value for the server to have time to handle the connection
            Thread.Sleep(1000);

            Runtime = new AutosplitterRuntime();

            Runtime.DoStartup();

            refreshTimer.Interval = 1000f / Runtime.Autosplitter.refreshRate;
            refreshTimer.AutoReset = true;

            refreshTimer.Elapsed += (o, e) => 
            {
                refreshTimer.Stop();
                Runtime.Update();
                refreshTimer.Start();
            };

            refreshTimer.Start();

            while(true)
            {
                CLI.HandleInput();
            }
        }

        public static string ReceiveFromServer()
        {
            byte[] buffer = new byte[16];
            string response = "";

            int bytesReceived;
            do
            {
                bytesReceived = socket.Receive(buffer, buffer.Length, 0);
                response += System.Text.UTF8Encoding.UTF8.GetString(buffer);
                if(response.Contains("\r\n")) { response = response.Split("\r\n")[0]; break; }
            }
            while(bytesReceived != 0);

            return response;
        }
    }
}
