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

        List<HandleClient> clientList;

        static void Main(string[] args)
        {
            Program myProgram = new Program();
            myProgram.Run();
        }

        public void Run()
        {
        TcpListener _serverSocket = new TcpListener(IPAddress.Any, 20000);
        TcpClient _clientSocket = default(TcpClient);
        int counter = 0;
        clientList = new List<HandleClient>();

        _serverSocket.Start();
        Console.WriteLine(" >> Server Started");
        Thread gavelThread = new Thread(HandleClient.Gavel);

        while (true)
        {
            counter += 1;
            _clientSocket = _serverSocket.AcceptTcpClient();
            Console.WriteLine(" >> Client No: " + counter + " Started!");
            HandleClient client = new HandleClient(_clientSocket, counter);
            HandleClient._clientList.Add(client);
            client.StartClient();
            
            if (counter == 1)
            {
                gavelThread.Start();
            }
        }
}

        public class HandleClient
        {
            public static bool newHighestBid = false;
            private TcpClient clientSocket;
            private string clNo;
            static int _currentBid = 0;
            static List<int> _previousBids = new List<int>();
            public static List<HandleClient> _clientList = new List<HandleClient>();
            private object _lock;
            

            internal HandleClient(TcpClient inClientSocket, int clientNo)
            {
                this.clientSocket = inClientSocket;
                clNo = clientNo.ToString();
            }

            public static void Gavel()
            {
                for ( int i = 30; i > 0; i--)
                {
                    if (newHighestBid)
                    {
                        i = 30;
                        foreach (var item in _clientList)
                        {
                            item.GavelMessage(i);
                        }
                        newHighestBid = false;
                    }
                    if (i == 5 || i == 3 || i == 1)
                    {
                        foreach (var item in _clientList)
                        {
                            item.GavelMessage(i);
                        }
                    }
                    
                        
                    Thread.Sleep(1000);

                }
            }

            internal void StartClient()
            {
                Thread newClient = new Thread(ClientHandler);
                newClient.Start();
            }

            internal void GavelMessage(int i)
            {
                NetworkStream stream = new NetworkStream(clientSocket.Client);
                StreamReader reader = new StreamReader(stream);
                StreamWriter writer = new StreamWriter(stream);
                writer.AutoFlush = true;
                writer.WriteLine();
                if (i == 30)
                {
                    writer.WriteLine("There has been a new bid. The gavel has restarted");
                }
                if (i == 5)
                {
                    writer.WriteLine("First");
                }
                if (i == 3)
                {
                    writer.WriteLine("Second");
                }
                if (i == 1)
                {
                    writer.WriteLine("THIRD! SOLD to highest bidder");
                }
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
                                    Monitor.Enter(_lock);
                                        newHighestBid = true;
                                        _previousBids.Add(_currentBid);
                                        _currentBid = i;
                                        Console.WriteLine(i + " is the highest bid ");
                                    Monitor.Exit(_lock);
                                }
                                else
                                {
                                    writer.WriteLine(_currentBid);
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
