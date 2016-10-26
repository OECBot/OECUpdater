using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace OECLib.Utilities
{
    public class CallBackServer
    {

        private TcpListener listener;

        public CallBackServer(String ip, int port)
        {
            listener = new TcpListener(IPAddress.Parse(ip), port);
            Console.WriteLine("Server listening at {0} : {1}", ip, port);
        }

        public void Start()
        {
            listener.Start();
            listener.BeginAcceptTcpClient(onAcceptConnection, null);
        }

        private void onAcceptConnection(IAsyncResult asyn)
        {
            TcpClient client = listener.EndAcceptTcpClient(asyn);
            Console.WriteLine("Client connected.");

            byte[] rq = new byte[8192];
            NetworkStream stream = client.GetStream();

            stream.Read(rq, 0, 8192);

            byte[] resp;

            if (!Encoding.UTF8.GetString(rq).Split(' ')[1].Contains("callback"))
            {
                resp = Encoding.UTF8.GetBytes("Hello There!");
                client.GetStream().Write(resp, 0, resp.Length);
                client.GetStream().Flush();
                client.Close();
                Console.WriteLine("Non-Callback detected. Nulling request.");
                listener.BeginAcceptTcpClient(onAcceptConnection, null);
                return;
            }

            String code = Encoding.UTF8.GetString(rq).Split(' ')[1].Split('=')[1].Split('&')[0];

            resp = Encoding.UTF8.GetBytes("Authorization successful. You can now close this web page.\n" + code);

            client.GetStream().Write(resp, 0, resp.Length);
            client.GetStream().Flush();

            Console.WriteLine("Response Sent");

            Console.WriteLine("Obtained Token: {0}", code);
            Console.WriteLine("Terminating Client & Listener...");
            client.Close();
            listener.Stop();
        }
    }
}
