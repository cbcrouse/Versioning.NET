using System.Collections.Generic;

namespace Domain.Entities
{
    /// <summary>
    /// Represents a git commit record.
    /// </summary>
    public class GitCommit
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="id">The commit identifier.</param>
        /// <param name="subject">The commit message subject line.</param>
        public GitCommit(string id, string subject)
        {
            Id = id;
            Subject = subject;
        }

        /// <summary>
        /// The unique identifier for the commit.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// The subject line of the commit.
        /// </summary>
        public string Subject { get; }

        /// <summary>
        /// A collection of associated file changes for the commit.
        /// </summary>
        public List<GitCommitFileInfo> Files { get; } = new List<GitCommitFileInfo>();
    }
}
