﻿using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OECLib.GitHub
{
    public class RepositoryManager
    {
        private Session session;
        public Repository repo { get; set; }
        private Dictionary<String, String> shaKeys;
        private Random r;

        public RepositoryManager(Session session, Repository repo)
        {
            this.session = session;
            this.repo = repo;
            this.shaKeys = new Dictionary<string, string>();
            r = new Random();
        }

        public async Task<String> getFile(String filePath)
        {
            var contentCollection = await session.client.Repository.Content.GetAllContents(repo.Id, filePath);
            if (contentCollection.Count == 0)
            {
                throw new Exception(String.Format("File: {0} not found in {1}", filePath, repo.Name));
            }
            var content = contentCollection[0];
            shaKeys[content.Name] = content.Sha;
            return content.Content;
        }

        public async Task<IReadOnlyList<PullRequest>> getAllPullRequests()
        {
            var prs = await session.client.PullRequest.GetAllForRepository(repo.Id, new ApiOptions());
            return prs;
        }

        public async Task updateFile(String filePath, String content, String branch)
        {
            UpdateFileRequest ufr = new UpdateFileRequest("Update "+filePath, content, shaKeys[filePath], branch);
            var changeSet = await session.client.Repository.Content.UpdateFile(repo.Id, filePath, ufr);
            Console.WriteLine(changeSet.Content);
            shaKeys.Remove(filePath);
        }

        public async Task<String> createBranch(String name)
        {
            var master = await session.client.Git.Reference.Get(repo.Id, "heads/master");
            await session.client.Git.Reference.Create(repo.Id, new NewReference("refs/heads/" + name, master.Object.Sha));
            return name;
        }

        public async Task createPullRequest(String title, String branch, String content)
        {
            NewPullRequest newPullRequest = new NewPullRequest(title, branch, "master");
            newPullRequest.Body = content;
            PullRequest pr = await session.client.PullRequest.Create(repo.Id, newPullRequest);
            
        }

        public async Task BeginCommitAndPull(String fileName, String content, String source)
        {
            String branch = await createBranch(fileName + getRandString(5));
            await updateFile(fileName, content, branch);
            await createPullRequest(branch, branch, source);
        }

        private String getRandString(int length)
        {
            String result = "";
            String a = "abcdefghijklmnopqrstuvwxyz1234567890";
            
            for (int i = 0; i < r.Next(1, length); i++)
            {
                int ran = r.Next(36);

                result += a[ran];
            }
            return result;
        }
    }
}