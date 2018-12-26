using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Timers;
namespace BotClients
{
	public class Program
	{
		static int port_ = FreeTcpPort();
		static UdpClient client = new UdpClient(port_);
		static IPEndPoint ep = new IPEndPoint(IPAddress.Broadcast, port_); // endpoint where server is listening

	
		public static void  Main(string[] args)
		{
			
			///int port2 = ((IPEndPoint)client.Client.LocalEndPoint).Port;
			Console.WriteLine("Bot is listening on port " + port_);
			//Console.Write("receive data from " + ep.ToString());
			while (true)
			{

			
				byte[] int_bytes = BitConverter.GetBytes((Int16)ep.Port);
				//if (BitConverter.IsLittleEndian)
				//{
				//	Array.Reverse(int_bytes);
				//	int_bytes[0] = int_bytes[2];
				//	int_bytes[1] = int_bytes[3];
				//}
				// send data
				client.Send(int_bytes, int_bytes.Length, new IPEndPoint(IPAddress.Broadcast, 31337) );

				// then receive data
				//

				//
				/*
				try
				{
					byte[] receivedData = client.Receive(ref ep);
					Console.WriteLine("bot " + receivedData);
				}
				catch(Exception e)
				{
					Console.WriteLine("exp " + e.Message);
				}
				*/
				Thread.Sleep(1000);
			}
		
		}
		public static void attack(int port_vic)
		{
			
			Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			IPAddress[] ipnew = Dns.GetHostAddresses(Dns.GetHostName());
			IPEndPoint ipEnd = new IPEndPoint(ipnew[1], port_vic);
			s.Connect(ipEnd);
			
			s.Send(System.Text.Encoding.ASCII.GetBytes("hack"));
		
			
			Console.ReadLine();
			s.Close();
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