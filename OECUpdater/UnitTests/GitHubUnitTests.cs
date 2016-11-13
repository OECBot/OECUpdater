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
        private Session s2;
        private RepositoryManager rm;
        private Random r;

        [SetUp]
        protected void SetUp()
        {
            session = new Session("OECBot", "UoJ84XJTXphgO4F");
            s2 = new Session("SpazioTest", "duFe=rabeh8f");
            Task<Repository> repo = session.client.Repository.Get("Gazing", "OECTest");
            repo.Wait();
            rm = new RepositoryManager(session, repo.Result);
            r = new Random();
        }

        [Test]
        public void SessionStringTest()
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

        [TestCase("OECTest", "Gazing", true)]
        [TestCase("SpazioTest", "OECBot", false)]
        [TestCase("open_exoplanet_catalogue", "OpenExoplanetCatalogue", false)]
        public void IsCollaboratorTest(String name, String owner, bool isCollab)
        {
            Task<User> current = s2.client.User.Current();
            current.Wait();
            Task<bool> result = session.CheckAccess(current.Result.Id, owner, name);
            result.Wait();
            Assert.AreEqual(result.Result, isCollab);
        }

        [Test]
        public void GetFileTest()
        {
            Exception ex = Assert.ThrowsAsync<NotFoundException>(async () => await rm.getFile("systems/randomfile.xml"));
        }

        [Test]
        public void PullRequestTest()
        {
            Task<IReadOnlyList<PullRequest>> prs = rm.getAllPullRequests();
            prs.Wait();
            Assert.AreEqual(prs.Result.Count, 0);
        }

        [Test]
        public void CreateBranchTest()
        {
            Task<String> branch = rm.createBranch(getRandString(10));
            branch.Wait();
            Assert.DoesNotThrowAsync(async () => await session.client.Repository.Branch.Get(rm.repo.Id, branch.Result));         
        }

        [Test]
        public void UpdateFileTest()
        {
            Task<String> getFile = rm.getFile("test.txt");
            getFile.Wait();
            String test = getRandString(10);
            Assert.DoesNotThrowAsync(async () => await rm.updateFile("test.txt", test, "master"));
            getFile = rm.getFile("test.txt");
            getFile.Wait();
            Assert.AreEqual(getFile.Result, test);
        }

        [Test]
        public void CreateFileTest()
        {
            String fileName = getRandString(7)+".txt";
            String content = getRandString(1337);
            Assert.DoesNotThrowAsync(async () => await rm.addFile(fileName, content, "master"));
            Task<String> getFile = rm.getFile(fileName);
            getFile.Wait();
            Assert.AreEqual(getFile.Result, content);
        }

        public String getRandString(int length)
        {
            String result = "";
            String a = "abcdefghijklmnopqrstuvwxyz1234567890";

            for (int i = 0; i < length; i++)
            {
                int ran = r.Next(36);

                result += a[ran];
            }
            return result;
        }
    }
}
