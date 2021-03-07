using Domain.Entities;
using LibGit2Sharp;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Infrastructure.Extensions
{
    /// <summary>
    /// Provides extended functionality for <see cref="Commit"/>.
    /// </summary>
    public static class CommitExtensions
    {
        /// <summary>
        /// Returns a collection of associated file updates for the commit.
        /// </summary>
        /// <param name="commit">The git commit.</param>
        /// <param name="repo">The git repository.</param>
        public static IEnumerable<GitCommitFileInfo> GetFileUpdates(this Commit commit, Repository repo)
        {
            var updates = new List<GitCommitFileInfo>();
            var parentDirectory = Directory.GetParent(repo.Info.Path)!.Parent!.FullName;

            // Merge commits do not affect the amount of files updated
            foreach (Commit repoCommitParent in commit.Parents)
            {
                updates.AddRange(repo.Diff.Compare<TreeChanges>(repoCommitParent.Tree, commit.Tree)
                    .Select(treeEntryChanges => new GitCommitFileInfo(parentDirectory, treeEntryChanges.Path)));
            }

            // The initializing commit will not have a parent
            if (!commit.Parents.Any())
            {
                TreeChanges treeEntryChanges = repo.Diff.Compare<TreeChanges>(null, commit.Tree);
                updates.AddRange(treeEntryChanges
                    .Select(treeEntryChange => new GitCommitFileInfo(parentDirectory, treeEntryChange.Path)));
            }

            return updates;
        }

        /// <summary>
        /// Converts a <see cref="Commit"/> to the domain <see cref="GitCommit"/>.
        /// </summary>
        /// <param name="commit"></param>
        /// <param name="repo"></param>
        /// <returns></returns>
        public static GitCommit ToDomain(this Commit commit, Repository repo)
        {
            return new GitCommit(commit.Id.ToString(), commit.MessageShort, commit.GetFileUpdates(repo));
        }
    }
}
