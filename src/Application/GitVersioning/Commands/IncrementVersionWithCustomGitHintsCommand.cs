using Domain.Enumerations;
using MediatR;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Application.GitVersioning.Commands
{
    /// <summary>
    /// The <see cref="IRequest{TResponse}"/> object responsible for incrementing the version with git integration, with custom hints.
    /// </summary>
    public class IncrementVersionWithCustomGitHintsCommand : IRequest<Unit>
    {
        /// <summary>
        /// Gets or sets the directory containing the .git folder.
        /// </summary>
        public string GitDirectory { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the directory to target for file versioning.
        /// </summary>
        public string TargetDirectory { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the search option to use with the <see cref="TargetDirectory"/>.
        /// </summary>
        public SearchOption SearchOption { get; set; } = SearchOption.AllDirectories;

        /// <summary>
        /// Gets or sets the author email to use when creating a commit.
        /// </summary>
        public string CommitAuthorEmail { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the git remote target. Defaults to 'origin'.
        /// </summary>
        public string RemoteTarget { get; set; } = "origin";

        /// <summary>
        /// Gets or sets the name of the branch to update.
        /// </summary>
        public string BranchName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets whether beta mode should be exited.
        /// </summary>
        public bool? ExitBeta { get; set; } = null;

        /// <summary>
        /// Gets or sets the prefix value to use for the version tag in git. Defaults to 'v'.
        /// </summary>
        public string TagPrefix { get; set; } = "v";

        /// <summary>
        /// Gets or sets the suffix value to use for the version tag in git.
        /// </summary>
        public string TagSuffix { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the list of commit's types how will create a none increment
        /// </summary>
        public IEnumerable<string> SkipTypeHints { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the list of commit's scopes how will create a none increment
        /// </summary>
        public IEnumerable<string> SkipScopeHints { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the list of commit's subjects hints how will create a none increment
        /// </summary>
        public IEnumerable<string> SkipSubjectHints { get; set; } = new List<string> { "[skip hint]" };

        /// <summary>
        /// Gets or sets the list of commit's types how will create a major increment
        /// </summary>
        public IEnumerable<string> MajorTypeHints { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the list of commit's scopes how will create a major increment
        /// </summary>
        public IEnumerable<string> MajorScopeHints { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the list of commit's subjects hints how will create a major increment
        /// </summary>
        public IEnumerable<string> MajorSubjectHints { get; set; } = new List<string> { "#breaking" };

        /// <summary>
        /// Gets or sets the list of commit's types how will create a minor increment
        /// </summary>
        public IEnumerable<string> MinorTypeHints { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the list of commit's scopes how will create a minor increment
        /// </summary>
        public IEnumerable<string> MinorScopeHints { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the list of commit's subjects hints how will create a minor increment
        /// </summary>
        public IEnumerable<string> MinorSubjectHints { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the list of commit's types how will create a patch increment
        /// </summary>
        public IEnumerable<string> PatchTypeHints { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the list of commit's scopes how will create a patch increment
        /// </summary>
        public IEnumerable<string> PatchScopeHints { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the list of commit's subjects hints how will create a patch increment
        /// </summary>
        public IEnumerable<string> PatchSubjectHints { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the case insensitive for the git hints
        /// </summary>
        public bool CaseInsensitiveHints { get; set; } = false;
    }
}
