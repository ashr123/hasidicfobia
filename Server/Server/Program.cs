using System;
using System.Net;      //required
using System.Net.Sockets;    //required
using System.Text;

namespace Server
{
	public class Program
	{
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
			IPHostEntry IPHost = Dns.GetHostEntry(Dns.GetHostName());

			Console.WriteLine(Dns.GetHostName());
			foreach (IPAddress ip in IPHost.AddressList)
				Console.WriteLine(ip);

			//TcpListener server = new TcpListener(IPAddress.Any, 11000);
			//// we set our IP address as server's address, and we also set the port: 9999

			//server.Start();  // this will start the server

			//while (true)   //we wait for a connection
			//{
			//	TcpClient client = server.AcceptTcpClient();  //if a connection exists, the server will accept it

			//	NetworkStream ns = client.GetStream(); //networkstream is used to send/receive messages

			//	//byte[] hello = new byte[100];   //any message must be serialized (converted to byte array)
			//	byte[] hello = Encoding.Default.GetBytes("hello world");  //conversion string => byte array

			//	ns.Write(hello, 0, hello.Length);     //sending the message

			//	while (client.Connected)  //while the client is connected, we look for incoming messages
			//	{
			//		byte[] msg = new byte[1024];     //the messages arrive as byte array
			//		ns.Read(msg, 0, msg.Length);   //the same networkstream reads the message sent by the client
			//		Console.WriteLine(Encoding.Default.GetString(msg).Trim()); //now , we write the message as string
			//	}
			//}

			UdpClient udpServer = new UdpClient(11000);

			while (true)
			{
				IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 11000);
				byte[] data = udpServer.Receive(ref remoteEP); // listen on port 11000
				Console.Write("receive data from " + remoteEP.ToString());
				udpServer.Send(new byte[] { 1 }, 1, remoteEP); // reply back
			}

		}
	}
}