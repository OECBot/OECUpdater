using Octokit;
using OECLib.GitHub;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace OECLib.Utilities
{
    public class CallBackServer
    {

        private TcpListener listener;
        public String Code;
        public Session session;
        private GitHubClient GHClient;
		private CancellationTokenSource cts;

        public CallBackServer(String ip, int port, GitHubClient client)
        {
            listener = new TcpListener(IPAddress.Parse(ip), port);
            Console.WriteLine("Server listening at {0}:{1}", ip, port);
            this.GHClient = client;
			cts = new CancellationTokenSource();
        }

        public async Task<Session> Start()
        {
            listener.Start();

            bool gotCode = false;
            do
            {
				var token = cts.Token;
				TcpClient client = await Task.Run(() => listener.AcceptTcpClientAsync(), token);
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
                    continue;
                }

                Code = Encoding.UTF8.GetString(rq).Split(' ')[1].Split('=')[1].Split('&')[0];

                resp = Encoding.UTF8.GetBytes("Authorization successful. You can now close this web page.");

                client.GetStream().Write(resp, 0, resp.Length);
                client.GetStream().Flush();

                Console.WriteLine("Obtained Token: {0}", Code);
                Console.WriteLine("Terminating Client & Listener...");
                client.Close();
                session = new Session(Code, GHClient);
                await session.ObtainNewToken();
                gotCode = true;
            } while (!gotCode);

            return session;
        }

		public void Stop() {
			cts.Cancel ();
		}
    }
}
