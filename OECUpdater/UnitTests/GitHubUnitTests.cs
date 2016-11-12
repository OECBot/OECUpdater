using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using OECLib.GitHub;
using Octokit;

namespace UnitTests
{
    [TestFixture]
    public class GitHubUnitTests
    {
        private Session session;

        [SetUp]
        protected void SetUp()
        {
           //session = new Session("test", "test");
        }

        [Test]
        public void SessionTypeTest()
        {
            Session normalSession = new Session("username", "password");
            String actual = String.Format("Session ({1}): Username: {0} - Password: {2}", "username", AuthenticationType.Basic, "password");
            Assert.AreEqual(normalSession.ToString(), actual);
        }

        [Test]
        public void ValidSessionTypeTest()
        {
            Session normalSession = new Session("username", "password");
            Assert.ThrowsAsync<InvalidOperationException>(async () => await normalSession.ObtainNewToken());
        }

    }
}
