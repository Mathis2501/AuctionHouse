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
            TcpListener serverSocket = new TcpListener(IPAddress.Any, 20000);
            TcpClient clientSocket = default(TcpClient);
            int counter = 0;

            serverSocket.Start();
            Console.WriteLine(" >> Server Started");

            while (true)
            {
                counter += 1;
                clientSocket = serverSocket.AcceptTcpClient();
                Console.WriteLine(" >> Client No: " + counter + " Started!");
                HandleClient client = new HandleClient();
                client.StartClient(clientSocket, counter);
            }

            clientSocket.Close();
            serverSocket.Stop();
            Console.WriteLine(" >> " + "exit");
            Console.ReadLine();
        }

        public class HandleClient
        {
            private TcpClient clientSocket;
            private string clNo;

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
                        Console.WriteLine("I am connected to " + remoteIpEndPoint.Address + " on port number " +
                                          remoteIpEndPoint.Port);
                    }

                    if (localIpEndPoint != null)
                    {
                        Console.WriteLine("My local IpAddress is :" + localIpEndPoint.Address +
                                          " I am connected on port number " + localIpEndPoint.Port);
                    }

                    if (reader.ReadLine().ToLower() == "hello server")
                    {
                        writer.WriteLine("Hello, Client!");
                    }


                    while (clientSocket.Client.Connected)
                    {
                        Console.WriteLine(reader.ReadLine());
                    }


                }
            }
}
