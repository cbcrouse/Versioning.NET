using Domain.Entities;
using System.Collections.Generic;

namespace Application.Interfaces
{
    /// <summary>
    /// An abstraction to facilitate testing without using the git integration.
    /// </summary>
    public interface IGitService
    {
        /// <summary>
        /// Retrieve a collection of git commits.
        /// </summary>
        /// <param name="gitDirectory">The directory containing the .git folder.</param>
        public List<GitCommit> GetCommits(string gitDirectory);

        /// <summary>
        /// Retrieve a collection of filtered commits.
        /// </summary>
        /// <param name="filter">A filter object for git commit queries.</param>
        /// <param name="gitDirectory">The directory containing the .git folder.</param>
        public List<GitCommit> GetCommitsByFilter(string gitDirectory, GitCommitFilter filter);

        /// <summary>
        /// Retrieve the hash identifier for the tag.
        /// </summary>
        /// <param name="gitDirectory">The directory containing the .git folder.</param>
        /// <param name="tagValue">The value of the tag.</param>
        public string GetTagId(string gitDirectory, string tagValue);

        /// <summary>
        /// Retrieve the hash identifier for the commit at the tip of the branch.
        /// </summary>
        /// <param name="gitDirectory">The directory containing the .git folder.</param>
        /// <param name="remoteTarget">The git target location identifier. Typically this value is 'origin'.</param>
        /// <param name="branchName">The name of the branch.</param>
        public string GetBranchTipId(string gitDirectory, string remoteTarget, string branchName);

        /// <summary>
        /// Retrieve a collection of git tags.
        /// </summary>
        /// <param name="gitDirectory">The directory containing the .git folder.</param>
        public IEnumerable<string> GetTags(string gitDirectory);

        /// <summary>
        /// Stage current changes and create a git commit with the specified message.
        /// </summary>
        /// <param name="gitDirectory">The directory containing the .git folder.</param>
        /// <param name="commitMessage">The message for the git commit.</param>
        /// <param name="authorEmail">The e-mail for the commit author.</param>
        public void CommitChanges(string gitDirectory, string commitMessage, string authorEmail);

        /// <summary>
        /// Create a tag on a specific commit.
        /// </summary>
        /// <param name="gitDirectory">The directory containing the .git folder.</param>
        /// <param name="tagValue">The value of the tag.</param>
        /// <param name="commitId">The unique identifier of the commit.</param>
        public void CreateTag(string gitDirectory, string tagValue, string commitId);

        /// <summary>
        /// Push a tag or branch to remote.
        /// </summary>
        /// <param name="gitDirectory">The directory containing the .git folder.</param>
        /// <param name="remoteTarget">The git target location identifier. Typically this value is 'origin'.</param>
        /// <param name="pushRefSpec">The pushRefSpec to push.</param>
        public void PushRemote(string gitDirectory, string remoteTarget, string pushRefSpec);
    }
}
