using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

using System.Net;      //required
using System.Net.Sockets;    //required
namespace VictemApp
{
	class Program
	{
		private static string pass = CreatePassword(6);
		private static List<bool> connectedBots = new List<bool>();

		static void Main(string[] args)
		{
			int port = FreeTcpPort();
			Console.WriteLine("Server listening on port " + port + ", password is " + pass);
			TcpListener server = new TcpListener(IPAddress.Any, port);
			// we set our IP address as server's address, and we also set the port: 9999

			server.Start();  // this will start the server

			while (true)   //we wait for a connection
			{
			
				TcpClient client = server.AcceptTcpClient();  //if a connection exists, the server will accept it

				NetworkStream ns = client.GetStream(); //networkstream is used to send/receive messages

				//byte[] hello = new byte[100];   //any message must be serialized (converted to byte array)
				// hello = Encoding.Default.GetBytes("hello world");  //conversion string => byte array
				byte[] enterPass = Encoding.Default.GetBytes("Please enter password\r\n");
				ns.Write(enterPass, 0, enterPass.Length);     //sending the message
				Console.WriteLine("sent from bot: Please enter password\r\n");
				byte[] msg = new byte[256];     //the messages arrive as byte array

				while (client.Connected)  //while the client is connected, we look for incoming messages
				{
					try
					{
						ns.Read(msg, 0, msg.Length);
						string msgstring = Encoding.Default.GetString(msg).Trim('\0');//the same networkstream reads the message sent by the client
						Console.Write("got from bot: " + msgstring);
						if (msgstring.Substring(0, 6).Equals(pass))
						{
							byte[] accessGranted = Encoding.Default.GetBytes("Access granted\r\n");
							ns.Write(accessGranted, 0, accessGranted.Length);
							Console.Write("sent from bot: Access granted\r\n");
						}
						else if (msgstring.Substring(0, 10).Equals("Hacked by "))
						{
							int index = connectedBots.Count;
							connectedBots.Add(true);
							if (GetActiveBots() < 10)
							{
								int time = 60 * 1000;
								Timer timer = new Timer(time);
								timer.Elapsed += (Object source, ElapsedEventArgs e) => connectedBots[index] = false;
								timer.Start();

							}
							else
							{
								Console.WriteLine(msgstring);
							}
							client.Close();
						}
						else
							client.Close();

						//Console.WriteLine(Encoding.Default.GetString(msg).Trim()); //now , we write the message as string

					}
					catch (Exception e)
					{
						Console.WriteLine(e.Message);
					}
					
				}
			}
		}

		private static string CreatePassword(int length)
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

		private static int FreeTcpPort()
		{
			TcpListener l = new TcpListener(IPAddress.Loopback, 0);
			l.Start();
			int port = ((IPEndPoint)l.LocalEndpoint).Port;
			l.Stop();
			return port;
		}

		private static int GetActiveBots()
		{
			return connectedBots.Where(p => p).Count();
		}
	}
}