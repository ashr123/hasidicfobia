using System;
using System.Net;
using System.Net.Sockets;

namespace BotClients
{
	public class Program
	{
		public static void Main(string[] args)
		{
			UdpClient client = new UdpClient();
			IPEndPoint ep = new IPEndPoint(IPAddress.Parse("10.0.2.15"), 11000); // endpoint where server is listening
			client.Connect(ep);

			// send data
			client.Send(new byte[] { 1, 2, 3, 4, 5 }, 5);

			// then receive data
			byte[] receivedData = client.Receive(ref ep);

			Console.Write("receive data from " + ep.ToString());

			Console.Read();
		}
	}
}