using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace DatadventuresServer
{
    class ThreadServer
    {
        TcpClient localClient;
        Thread t;
        int nthClient;
        List<Byte[]> bytes;

        public ThreadServer(TcpClient tcp, int connectionNumber, List<Byte[]> bytesCopy)
        {
            localClient = tcp;
            nthClient = connectionNumber;
            t = new Thread(handleData);
            t.Start();
            bytes = bytesCopy;
        }

        public void handleData()
        {
            Console.WriteLine("Connection #" +
                nthClient +
                "established. Client is at " + 
                localClient.Client.LocalEndPoint);

            var stream = localClient.GetStream();
            StreamReader s = new StreamReader(stream);
            while (true)
            {
                string str = s.ReadLine();
                if (str != null && str.Equals("DownloadGame"))
                {
                    using (StreamWriter w = new StreamWriter(stream)) {
                        foreach (Byte[] b in bytes)
                        {
                            stream.Write(b, 0, b.Length);
                        }
                    }
                    break;
                }
            }
            Console.WriteLine("Finished transferring.");
        }
    }
}
