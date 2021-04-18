using Domain.Enumerations;
using System;

namespace Domain.Entities
{
    /// <summary>
    /// A filter object for git commit queries.
    /// </summary>
    public class GitCommitFilter
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="fromHash">The commit identifier for the beginning of the included commits.</param>
        /// <param name="untilHash">The commit identifier for the non-inclusive final commit.</param>
        /// <param name="sortMode">Indicates the sort mode.</param>
        public GitCommitFilter(string fromHash, string untilHash, GitCommitSortMode sortMode = GitCommitSortMode.None)
        {
            if (string.IsNullOrEmpty(fromHash)) throw new ArgumentException(fromHash);
            if (string.IsNullOrEmpty(untilHash)) throw new ArgumentException(untilHash);

            FromHash = fromHash;
            UntilHash = untilHash;
            SortMode = sortMode;
        }

        /// <summary>
        /// Indicates the sort mode.
        /// </summary>
        public GitCommitSortMode SortMode { get; init; }

        /// <summary>
        /// The commit identifier for the beginning of the included commits.
        /// Commits are included starting at and after this commit.
        /// </summary>
        public string FromHash { get; init; }

        /// <summary>
        /// The commit identifier for the non-inclusive final commit.
        /// Commits are included until this commit.
        /// </summary>
        public string UntilHash { get; init; }
    }
}
