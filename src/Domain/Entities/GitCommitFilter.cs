using Domain.Enumerations;

namespace Domain.Entities
{
    /// <summary>
    /// A filter object for git commit queries.
    /// </summary>
    public class GitCommitFilter
    {
        /// <summary>
        /// Indicates the sort mode.
        /// </summary>
        public GitCommitSortMode SortMode { get; set; }

        /// <summary>
        /// The commit identifier for the beginning of the included commits.
        /// Commits are included starting at and after this commit.
        /// </summary>
        public string FromHash { get; set; }

        /// <summary>
        /// The commit identifier for the non-inclusive final commit.
        /// Commits are included until this commit.
        /// </summary>
        public string UntilHash { get; set; }
    }
}
