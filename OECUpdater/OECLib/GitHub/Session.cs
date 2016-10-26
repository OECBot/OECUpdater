using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OECLib.GitHub
{
    public class Session
    {
        public const String clientId = "8c533ca43cbfba2e2268";
        public const String clientSecret = "fdfa25888fb0f815c248114fba5f7f26d834743f";

        private Dictionary<String, String> sessionDict = new Dictionary<string, string>();

        private OauthToken Token;
        private String Code;

        public GitHubClient client;

        public Session(String code, GitHubClient client)
        {
            this.client = client;
            this.Code = code;
        }

        public async Task ObtainNewToken()
        {
            var request = new OauthTokenRequest(clientId, clientSecret, Code);
            Token = await client.Oauth.CreateAccessToken(request);
            Console.WriteLine(Token.AccessToken);
        }

        public override string ToString()
        {
            return String.Format("Session: Authorization Code: {0} - Access Token: {1}", Code, Token.AccessToken);
        }
    }
}
