using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
// ReSharper disable AssignNullToNotNullAttribute

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
                HandleClient client = new HandleClient(_clientSocket, counter);
                client.StartClient();
            }
        }

        public class HandleClient
        {
            private TcpClient clientSocket;
            private string clNo;
            static int _currentBid = 0;
            static List<int> _previousBids = new List<int>();

            internal HandleClient(TcpClient inClientSocket, int clientNo)
            {
                this.clientSocket = inClientSocket;
                clNo = clientNo.ToString();
            }

            internal void StartClient()
            {
                Thread newClient = new Thread(ClientHandler);
                newClient.Start();
            }

            internal void ClientHandler()
            {
                try
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
                            Console.WriteLine("I am connected to " + remoteIpEndPoint.Address + " on port number " +
                                              remoteIpEndPoint.Port);
                        }

                        if (clientSocket.Connected)
                        {
                            Thread.CurrentThread.Name = reader.ReadLine();
                            Console.WriteLine(Thread.CurrentThread.Name);
                            writer.WriteLine("You can now bid on a special one of a kind authentic Mcnugget shaped like a Mcnugget");
                            while (clientSocket.Connected)
                            {
                                int i = int.Parse(reader.ReadLine());
                                if (i > _currentBid)
                                {
                                    _currentBid = i;
                                    Console.WriteLine(i + " is the highest");
                                }
                                else
                                {
                                    writer.WriteLine(_currentBid.ToString());
                                }
                            }
                        }
                    }
                }
                catch (IOException e)
                {
                    Console.WriteLine("An IOException occurred: " + e);
                }
            }
        }
    }
}
