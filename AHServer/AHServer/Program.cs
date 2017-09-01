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

        public List<HandleClient> clientList;

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


        _serverSocket.Start();
        Console.WriteLine(" >> Server Started");
        HandleClient HC = new HandleClient();
        Thread gavelThread = new Thread(HC.Gavel);
        gavelThread.Start();

        while (true)
        {
            counter += 1;
            _clientSocket = _serverSocket.AcceptTcpClient();
            Console.WriteLine(" >> Client No: " + counter + " Started!");
            HandleClient client = new HandleClient(_clientSocket, counter, clientList);
            client.StartClient();
                
            clientList.Add(client);
        }
}

        public class HandleClient
        {
            public static bool newHighestBid = false;
            private TcpClient clientSocket;
            private string clNo;
            static int _currentBid = 0;
            static List<int> _previousBids = new List<int>();
          

            internal HandleClient()
            {
                
            }

            internal HandleClient(TcpClient inClientSocket, int clientNo, clientlist)
            {
                this.clientSocket = inClientSocket;
                clNo = clientNo.ToString();
                
            }

            public void Gavel()
            {
                Program myProgram = new Program();
                
                for ( int i = 30; i > 0; i--)
                {
                    if (newHighestBid)
                    {
                        i = 30;
                        foreach (var item in myProgram.clientList)
                        {
                            item.GavelMessage(i);
                        }
                        newHighestBid = false;
                    }
                    foreach (var item in myProgram.clientList)
                    {
                        item.GavelMessage(i);
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
                                    newHighestBid = true;
                                    _currentBid = i;
                                    Console.WriteLine(i + " is the highest bid ");
                                }
                                else
                                {
                                    writer.WriteLine(_currentBid.ToString() + " is the highest bid");
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
