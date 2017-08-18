using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AHServer
{
    class Program
    {
        
        static void Main(string[] args)
        {
            TcpListener _serverSocket = new TcpListener(IPAddress.Any, 20000);
            TcpClient _clientSocket = default(TcpClient);
            int counter = 0;

            _serverSocket.Start();
            Console.WriteLine(" >> Server Started");

            while (true)
            {
                counter += 1;
                _clientSocket = _serverSocket.AcceptTcpClient();
                Console.WriteLine(" >> Client No: " + counter + " Started!");
                HandleClient client = new HandleClient();
                client.StartClient(_clientSocket, counter);
            }

            _clientSocket.Close();
            _serverSocket.Stop();
            Console.WriteLine(" >> " + "exit");
            Console.ReadLine();
        }

        public class HandleClient
        {
            private TcpClient clientSocket;
            private string clNo;
            static int _currentBid = 0;
            static List<int> _previousBids = new List<int>();

            internal void StartClient(TcpClient inClientSocket, int clientNo)
            {
                this.clientSocket = inClientSocket;
                clNo = clientNo.ToString();
                Thread newThread = new Thread(ClientHandler);
                newThread.Start();

            }

            internal void ClientHandler()
            {
                while (true)

                {
                    IPEndPoint remoteIpEndPoint = clientSocket.Client.RemoteEndPoint as IPEndPoint;
                    IPEndPoint localIpEndPoint = clientSocket.Client.LocalEndPoint as IPEndPoint;

                    NetworkStream stream = new NetworkStream(clientSocket.Client);
                    StreamReader reader = new StreamReader(stream);
                    StreamWriter writer = new StreamWriter(stream);
                    writer.AutoFlush = true;

                    if (remoteIpEndPoint != null)
                    {
                        Console.WriteLine("I am connected to " + remoteIpEndPoint.Address + " on port number " + remoteIpEndPoint.Port);
                    }

                    if (clientSocket.Connected)
                    {
                        writer.WriteLine("What is your name?");
                        Console.WriteLine(reader.ReadLine());
                        //Thread.CurrentThread.Name = name;
                        //Console.WriteLine(Thread.CurrentThread.Name);
                        writer.WriteLine("You can now bid on a special one of a kind authentic Mcnugget shaped like a Mcnugget");
                    }

                    while (clientSocket.Client.Connected)
                    {
                        _currentBid = int.Parse(reader.ReadLine());
                    }
                }
            }
        }
    }
}
