using Application.Interfaces;
using Domain.Entities;
using Domain.Enumerations;
using Infrastructure.Extensions;
using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure.Services
{
    /// <summary>
    /// An implementation of the Git API using Lib2GitSharp library.
    /// http://www.woodwardweb.com/git/getting_started_2.html
    /// </summary>
    public class LibGit2Service : IGitService
    {
        /// <summary>
        /// Retrieve a collection of git commits.
        /// </summary>
        /// <param name="gitDirectory">The directory containing the .git folder.</param>
        public List<GitCommit> GetCommits(string gitDirectory)
        {
            using var repo = new Repository(gitDirectory);
            var commits = new List<GitCommit>();

            foreach (Commit repoCommit in repo.Commits)
            {
                var updates = repoCommit.GetFileUpdates(repo);
                var commit = new GitCommit(repoCommit.Id.ToString(), repoCommit.MessageShort, updates);
                commits.Add(commit);
            }

            return commits;
        }

        /// <summary>
        /// Retrieve a collection of filtered commits.
        /// </summary>
        /// <param name="filter">A filter object for git commit queries.</param>
        /// <param name="gitDirectory">The directory containing the .git folder.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public List<GitCommit> GetCommitsByFilter(string gitDirectory, GitCommitFilter filter)
        {
            using var repo = new Repository(gitDirectory);

            var sortStrategy = CommitSortStrategies.None;

            switch (filter.SortMode)
            {
                case GitCommitSortMode.None:
                    break;
                case GitCommitSortMode.Descending:
                    sortStrategy = CommitSortStrategies.Reverse | CommitSortStrategies.Time;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(filter.SortMode));
            }

            var commitFilter = new CommitFilter
            {
                SortBy = sortStrategy,
                ExcludeReachableFrom = filter.UntilHash,
                IncludeReachableFrom = filter.FromHash
            };

            List<Commit> commits = repo.Commits.QueryBy(commitFilter).ToList();

            return commits.Select(commit => commit.ToDomain(repo)).ToList();
        }

        /// <summary>
        /// Retrieve the hash identifier for the tag.
        /// </summary>
        /// <param name="gitDirectory">The directory containing the .git folder.</param>
        /// <param name="tagValue">The value of the tag.</param>
        public string GetTagId(string gitDirectory, string tagValue)
        {
            using var repo = new Repository(gitDirectory);
            return repo.Tags[tagValue]?.PeeledTarget.Id.ToString() ?? string.Empty;
        }

        /// <summary>
        /// Retrieve the hash identifier for the commit at the tip of the branch.
        /// </summary>
        /// <param name="gitDirectory">The directory containing the .git folder.</param>
        /// <param name="branchName">The name of the branch.</param>
        public string GetBranchTipId(string gitDirectory, string branchName)
        {
            using var repo = new Repository(gitDirectory);
            return repo.Branches[branchName]?.Tip.Id.ToString() ?? string.Empty;
        }

        /// <summary>
        /// Retrieve a collection of git tags.
        /// </summary>
        /// <param name="gitDirectory">The directory containing the .git folder.</param>
        public IEnumerable<string> GetTags(string gitDirectory)
        {
            using var repo = new Repository(gitDirectory);

            return repo.Tags.Select(repoTag => repoTag.FriendlyName).ToList();
        }

        /// <summary>
        /// Stage current changes and create a git commit with the specified message.
        /// </summary>
        /// <param name="gitDirectory">The directory containing the .git folder.</param>
        /// <param name="commitMessage">The message for the git commit.</param>
        /// <param name="authorEmail">The e-mail for the commit author.</param>
        public void CommitChanges(string gitDirectory, string commitMessage, string authorEmail)
        {
            using var repo = new Repository(gitDirectory);

            RepositoryStatus status = repo.RetrieveStatus();
            var filePaths = status.Modified.Select(mods => mods.FilePath).ToList();
            foreach (var filePath in filePaths)
            {
                repo.Index.Add(filePath);
                repo.Index.Write();
            }

            var author = new Signature(authorEmail, authorEmail, DateTimeOffset.Now);
            var committer = author;

            _ = repo.Commit(commitMessage, author, committer);
        }

        /// <summary>
        /// Create a tag on a specific commit.
        /// </summary>
        /// <param name="gitDirectory">The directory containing the .git folder.</param>
        /// <param name="tagValue">The value of the tag.</param>
        /// <param name="commitId">The unique identifier of the commit.</param>
        public void CreateTag(string gitDirectory, string tagValue, string commitId)
        {
            using var repo = new Repository(gitDirectory);
            repo.ApplyTag(tagValue, commitId);
        }

        /// <summary>
        /// Push a tag or branch to remote.
        /// </summary>
        /// <param name="gitDirectory">The directory containing the .git folder.</param>
        /// <param name="remoteTarget">The git target location identifier. Typically this value is 'origin'.</param>
        /// <param name="pushRefSpec">The pushRefSpec to push.</param>
        public void PushRemote(string gitDirectory, string remoteTarget, string pushRefSpec)
        {
            using var repo = new Repository(gitDirectory);
            var remote = repo.Network.Remotes[remoteTarget];
            var options = new PushOptions();
            repo.Network.Push(remote, pushRefSpec, options);
        }
    }
}
