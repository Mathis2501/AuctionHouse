using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AHClient
{
    class Program
    {
        static int HighestBid = 0;
        static int newBid;

        TcpClient client = new TcpClient();
        private StreamReader reader;
        private StreamWriter writer;

        static void Main(string[] args)
        {
            Program myProgram = new Program();
            myProgram.Run();

        }

        private void Run()
        {
            Thread ServerWriter = new Thread(SendingLocalHighestBid);


            Console.WriteLine("Connecting...");
            client.Connect("10.140.67.62", 20000);
            Console.Clear();
            Console.WriteLine("Connected");


            NetworkStream stream = client.GetStream();
            writer = new StreamWriter(stream);
            reader = new StreamReader(stream);
            writer.AutoFlush = true;

            Console.WriteLine("what is your name: ");
            writer.WriteLine(Console.ReadLine());

            Console.WriteLine(reader.ReadLine());

            ServerWriter.Start();
            while (client.Connected)
            {
                
                int i;
                if (int.TryParse(Console.ReadLine(), out i))
                {
                    newBid = i;
                    if (newBid > HighestBid)
                    {
                        writer.WriteLine(newBid);
                        Console.WriteLine("Du har budt: " + newBid);
                    }
                    else
                    {
                        Console.WriteLine("Dit bud var ikke gyldigt");
                    }
                }
                else
                {
                    Console.WriteLine("Brug tal dummernik");
                }
                


            }
            client.Close();



        }

        public void SendingLocalHighestBid()
        {
            while (true)
            {
                Thread.Sleep(1000);
                //Console.WriteLine("update started");
                writer.WriteLine(HighestBid.ToString());
                int i = int.Parse(reader.ReadLine());
                if (HighestBid < i)
                {
                    Console.WriteLine("Det højeste bud er: " + i);
                }
                HighestBid = i;


            }
        }
    }
}

