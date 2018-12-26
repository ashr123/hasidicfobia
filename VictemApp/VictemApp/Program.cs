using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net;      //required
using System.Net.Sockets;    //required
namespace VictemApp
{
	class Program
	{
        static void Main(string[] args)
        {
			int port = FreeTcpPort();
			string pass = CreatePassword(6);

			Console.WriteLine("Server listening on port " + port+ ", password is "+pass);
		   TcpListener server = new TcpListener(IPAddress.Any, port);
            // we set our IP address as server's address, and we also set the port: 9999

            server.Start();  // this will start the server

            while (true)   //we wait for a connection
            {
                TcpClient client = server.AcceptTcpClient();  //if a connection exists, the server will accept it

                NetworkStream ns = client.GetStream(); //networkstream is used to send/receive messages

                //byte[] hello = new byte[100];   //any message must be serialized (converted to byte array)
               // hello = Encoding.Default.GetBytes("hello world");  //conversion string => byte array

                //ns.Write(hello, 0, hello.Length);     //sending the message

                while (client.Connected)  //while the client is connected, we look for incoming messages
                {
                    byte[] msg = new byte[1024];     //the messages arrive as byte array
                    ns.Read(msg, 0, 4);
                    string msgstring = Encoding.Default.GetString(msg).Trim('\0');//the same networkstream reads the message sent by the client
                    if (msgstring.Equals("hack")){
                        Console.WriteLine("i have been hacked");
                    }
                    Console.WriteLine(Encoding.Default.GetString(msg).Trim()); //now , we write the message as string
					
                }
                Console.ReadLine();
            }

        }
		public static string CreatePassword(int length)
		{
			const string valid = "abcdefghijklmnopqrstuvwxyz";
			StringBuilder res = new StringBuilder();
			Random rnd = new Random();
			while (0 < length--)
			{
				res.Append(valid[rnd.Next(valid.Length)]);
			}
			return res.ToString();
		}

		static int FreeTcpPort()
		{
			TcpListener l = new TcpListener(IPAddress.Loopback, 0);
			l.Start();
			int port = ((IPEndPoint)l.LocalEndpoint).Port;
			l.Stop();
			return port;
		}
	}
}
