﻿using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OECLib.Utilities;
using System.Threading;

namespace OECLib.GitHub
{
    public class Session
    {
        public const String clientId = "8c533ca43cbfba2e2268";
        public const String clientSecret = "fdfa25888fb0f815c248114fba5f7f26d834743f";
        public static ProductHeaderValue Header = new ProductHeaderValue("SpazioApp");

		public static CallBackServer server;
		public User current;
		private Dictionary<String, String> session = new Dictionary<string, string> ();

        private OauthToken Token;
        private String Code;

        public GitHubClient client { get; set; }

        public Session(String code, GitHubClient client)
        {
            this.client = client;
            this.Code = code;
        }

        public Session(Credentials cred)
        {
            this.client = new GitHubClient(Header);
            this.client.Credentials = cred;
        }

		public Session(String uname, String password)
		{
			this.client = new GitHubClient(Header);
			this.client.Credentials = new Credentials(uname, password);
		}

		public async static Task<Session> CreateOauthSession()
		{
			if (server != null) {
				server.Stop ();

				while (server.isRunning) {
					Console.WriteLine ("IS RUNNING STILL");
					await Task.Delay (100);
				}
			}
			GitHubClient g = new GitHubClient (Header);
			server = new CallBackServer ("127.0.0.1", 4567, g);


			var request = new OauthLoginRequest (Session.clientId) {
				Scopes = { "user", "notifications", "repo" }
			};
			try {
				String oauthLoginUrl = g.Oauth.GetGitHubLoginUrl (request).ToString ();
				System.Diagnostics.Process.Start (oauthLoginUrl);

				Session session = await server.Start ();

				return session;
			}
			catch (OperationCanceledException ex) {
				Console.WriteLine ("Callbackserver cancelled");
				Console.WriteLine (ex.Message);
				return null;
			}
		}

		public async Task SetCurrentUser() {
			if (client.Credentials.AuthenticationType == AuthenticationType.Oauth)
			{
				if (server.isCancelled) {
					return;
				}
			}
			current = await client.User.Current ();
		}

        public async Task ObtainNewToken()
        {
            if (client.Credentials.AuthenticationType == AuthenticationType.Basic)
            {
                throw new InvalidOperationException("Cannot obtain new token for non-oauth session!");
            }
            var request = new OauthTokenRequest(clientId, clientSecret, Code);
			if (server.isCancelled) {
				return;
			}
            Token = await client.Oauth.CreateAccessToken(request);
            session["OAuthToken"] = Token.AccessToken;
            client.Credentials = new Credentials(Token.AccessToken);
        }

        public override string ToString()
        {
            if (client.Credentials.AuthenticationType == AuthenticationType.Oauth)
            {
                return String.Format("Session ({2}): Authorization Code: {0} - Access Token: {1}", Code, Token.AccessToken, client.Credentials.AuthenticationType);
            }
            return String.Format("Session ({1}): Username: {0} - Password: {2}", client.Credentials.Login, client.Credentials.AuthenticationType, client.Credentials.Password);
        }

        public async Task<bool> CheckAccess(int id, String owner, String name)
        {
            try
            {
                var repos = await client.Repository.Collaborator.GetAll(owner, name);
                User cur = await client.User.Current();
                foreach (User person in repos)
                {
                    if (id == person.Id)
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            
            return false;
        }
    }
}
