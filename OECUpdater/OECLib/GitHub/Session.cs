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
        public static enum SessionType
        {
            BASIC_AUTH,
            OAUTH
        }

        public SessionType type;
        public const String clientId = "8c533ca43cbfba2e2268";
        public const String clientSecret = "fdfa25888fb0f815c248114fba5f7f26d834743f";

        private Dictionary<String, String> session = new Dictionary<string, string>();

        private OauthToken Token;
        private String Code;

        public GitHubClient client { get; set; }

        public Session(String code, GitHubClient client)
        {
            this.client = client;
            this.Code = code;
            this.type = SessionType.OAUTH;
        }

        public Session(Credentials cred)
        {
            this.client = new GitHubClient(new ProductHeaderValue("SpazioApp"));
            this.client.Credentials = cred;
            this.type = SessionType.BASIC_AUTH;
        }

        public async Task ObtainNewToken()
        {
            if (type != SessionType.OAUTH)
            {
                throw new InvalidOperationException("Cannot obtain new token for non-oauth session!");
            }
            var request = new OauthTokenRequest(clientId, clientSecret, Code);
            Token = await client.Oauth.CreateAccessToken(request);
            session["OAuthToken"] = Token.AccessToken;
            client.Credentials = new Credentials(Token.AccessToken);
        }

        public override string ToString()
        {
            return String.Format("Session: Authorization Code: {0} - Access Token: {1}", Code, Token.AccessToken);
        }
    }
}
