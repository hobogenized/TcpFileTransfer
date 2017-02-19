using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace DatadventuresServer
{
	class MainServer
	{
		private TcpListener listener;
		private const int PORT_NUMBER = 8888;
		List<Byte[]> bytes;
		public bool createThread;

		public MainServer(int port)
		{
			IPEndPoint e = new IPEndPoint(IPAddress.Any, port);
			listener = new TcpListener(e);
			bytes = new List<Byte[]>();
			createThread = true;
		}

		public async void handleNewConnection(int connectionNumber, List<Byte[]> bytes)
		{
			createThread = false;
			var task = listener.AcceptTcpClientAsync();
			TcpClient t = await task;
			ThreadServer s = new ThreadServer(t, connectionNumber, bytes);
			createThread = true;
		}

		static void Main(string[] args)
		{
			MainServer m = new MainServer(PORT_NUMBER);
			m.listener.Start();
			int connectionNumber = 0;
			using (FileStream fs = File.Open("data.tar", FileMode.Open))
			{
				long len = fs.Length;
				long count = 0;
				byte[] bytes = new byte[1024 * 8];
				while (count < len && (count = fs.Read(bytes, 0, bytes.Length)) > 0)
				{
					m.bytes.Add(bytes);
					bytes = new byte[1024 * 8];
				}
			}
			while (true)
			{
				if (m.createThread)
				{
					Console.WriteLine("Waiting for new connection");
					m.handleNewConnection(connectionNumber, m.bytes);
					connectionNumber++;
				}
			}
		}
	}
}
