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
		public Session session { get; set; }
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
            try
            {
                var contentCollection = await session.client.Repository.Content.GetAllContents(repo.Id, filePath);
                var content = contentCollection[0];
                shaKeys[filePath] = content.Sha;
                return content.Content;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<IReadOnlyList<PullRequest>> getAllPullRequests()
        {
            var prs = await session.client.PullRequest.GetAllForRepository(repo.Id);
            return prs;
        }

        public async Task updateFile(String filePath, String content, String branch)
        {
            UpdateFileRequest ufr = new UpdateFileRequest("Updated "+filePath, content, shaKeys[filePath], branch);
            var changeSet = await session.client.Repository.Content.UpdateFile(repo.Id, filePath, ufr);
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

        public async Task BeginCommitAndPush(String fileName, String content, String source, bool isNew)
        {
            String branch = await createBranch(fileName.Split('.')[0].Split('/')[1].Replace(' ', '_') +"_"+ getRandString(5));
            if (isNew)
            {
                await addFile(fileName, content, branch);
            }
            else
            {
                await updateFile(fileName, content, branch);
            }
			await Task.Delay (100);
            String body = String.Format("Updated {0}. Time Stamp: {1}\nSources:\n{2}", fileName.Split('.')[0].Split('/')[1], DateTime.Now.ToString(), source);
            await createPullRequest(branch, branch, body); 
        }

		public async Task<TreeResponse> getAllFiles()
        {
            //IReadOnlyList<RepositoryContent> files = await session.client.Repository.Content.GetAllContents(repo.Id, path);
			TreeResponse files = await session.client.Git.Tree.GetRecursive (repo.Id, "master");

            return files;
        }

		public async Task<int> getFileCount(String path) {
			var tree = await getAllFiles ();
			var iterator = tree.Tree.Where (x => x.Path.Contains ("systems/") == true);
			int count = 0;
			foreach (TreeItem item in iterator) {
				count++;
			}
			return count;
		}

        public async Task addFile(String filePath, String content, String branch)
        {
            CreateFileRequest cfr = new CreateFileRequest("Adding new " + filePath, content, branch);
            await session.client.Repository.Content.CreateFile(repo.Id, filePath, cfr);
        }

        public async Task deleteBranch(String branch)
        {
            await session.client.Git.Reference.Delete(repo.Id, "heads/"+branch);
        }

        public async Task<List<String>> getAllBranches()
        {
            IReadOnlyList<Branch> branches = await session.client.Repository.Branch.GetAll(repo.Id);
            List<String> branchNames = new List<String>();
            foreach (Branch branch in branches)
            {
                branchNames.Add(branch.Name);
            }
            return branchNames;
        }

        public async Task MergePullRequest(String branch, int prNum)
        {
            MergePullRequest mpr = new MergePullRequest();
            mpr.CommitMessage = String.Format("Merging branch {0} Timestamp: {1}", branch, DateTime.Now);
            mpr.CommitTitle = String.Format("Merge pullrequest for {0}", branch);
            await session.client.Repository.PullRequest.Merge(repo.Id, prNum, mpr);
        }

        public async Task closePullRequest(int prNum)
        {
            PullRequestUpdate pru = new PullRequestUpdate();
            pru.State = ItemState.Closed;
            await session.client.Repository.PullRequest.Update(repo.Id, prNum, pru);
        }

        private String getRandString(int length)
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
