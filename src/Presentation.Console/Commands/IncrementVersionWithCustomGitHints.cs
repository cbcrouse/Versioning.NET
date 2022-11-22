using Application.GitVersioning.Commands;
using McMaster.Extensions.CommandLineUtils;
using MediatR;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Presentation.Console.Commands
{
    /// <summary>
    /// Increment versions in csproj files with git integration based on git commit messages, with custom hint.
    /// </summary>
    [Command(Description = "Increment versions in csproj files with git integration based on git commit messages, with custom hint.")]
    public class IncrementVersionWithCustomGitHints
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="mediator">An abstraction for accessing application behaviors.</param>
#pragma warning disable 8618
        public IncrementVersionWithCustomGitHints(IMediator mediator)
#pragma warning restore 8618
        {
            _mediator = mediator;
            RemoteTarget = "origin";
            AuthorEmail = "tool@versioning.net";
            ExitBeta = null;
            TagPrefix = "v";
            TagSuffix = string.Empty;
            CaseInsensitiveHints = false;
        }

        /// <summary>
        /// The directory containing the .git folder.
        /// </summary>
        [Option(Description = "The directory containing the .git folder.")]
        [Required]
        public string GitDirectory { get; set; }

        /// <summary>
        /// The directory to use for file versioning. Defaults to the GitDirectory if not provided.
        /// </summary>
        [Option(ShortName = "d", Description = "The directory to use for file versioning. Defaults to the GitDirectory if not provided.")]
        public string TargetDirectory { get; set; } = string.Empty;

        /// <summary>
        /// The search option to use with the <see cref="TargetDirectory"/>. Defaults to <see cref="SearchOption.AllDirectories"/>.
        /// </summary>
        [Option(Description = "The search option to use with the target directory. Defaults to AllDirectories.")]
        [AllowedValues("AllDirectories", "TopDirectoryOnly", IgnoreCase = true)]
        public SearchOption SearchOption { get; set; } = SearchOption.AllDirectories;

        /// <summary>
        /// The git remote target. Defaults to 'origin'.
        /// </summary>
        [Option(ShortName = "t", Description = "The git remote target. Defaults to 'origin'.")]
        public string RemoteTarget { get; set; }

        /// <summary>
        /// The name of the branch to update.
        /// </summary>
        [Option(Description = "The name of the branch to update.")]
        [Required]
        public string BranchName { get; set; }

        /// <summary>
        /// The git commit author's email address.
        /// </summary>
        [Option(Description = "The git commit author's email address.")]
        public string AuthorEmail { get; set; }

        /// <summary>
        /// Determines whether beta mode should be exited.
        /// </summary>
        [Option(Description = "Determines whether beta mode should be exited. This will set the version to 1.0.0 if the version was lower.")]
        public bool? ExitBeta { get; set; }

        /// <summary>
        /// The prefix value to use for the version tag in git. Defaults to 'v'.
        /// </summary>
        [Option(ShortName = "tp", Description = "The prefix value to use for the version tag in git. Defaults to 'v'.")]
        public string TagPrefix { get; set; }

        /// <summary>
        /// The suffix value to use for the version tag in git.
        /// </summary>
        [Option(ShortName = "ts", Description = "The suffix value to use for the version tag in git.")]
        public string TagSuffix { get; set; }

        /// <summary>
        /// Determines whether git hints should be case-insensitive.
        /// </summary>
        [Option(ShortName = "ci", Description = "Determines whether git hints should be case-insensitive. Default to false")]
        public bool CaseInsensitiveHints { get; set; }

        /// <summary>
        /// Determines commit's types how will not create a increment.
        /// </summary>
        [Option(ShortName = "hst", Description = "Determines commit's types how will not create a increment.")]
        public string[] SkipTypeHints { get; set; } = System.Array.Empty<string>();

        /// <summary>
        /// Determines commit's scopes how will not create a increment.
        /// </summary>
        [Option(ShortName = "hsc", Description = "Determines commit's scopes how will not create a increment.")]
        public string[] SkipScopeHints { get; set; } = System.Array.Empty<string>();

        /// <summary>
        /// Determines commit's subject's hints how will not create a increment.
        /// </summary>
        [Option(ShortName = "hsu", Description = "Determines commit's subject's hints how will not create a increment.")]
        public string[] SkipSubjectHints { get; set; } = System.Array.Empty<string>();

        /// <summary>
        /// Determines commit's types how will create a major increment.
        /// </summary>
        [Option(ShortName = "hat", Description = "Determines commit's types how will create a major increment.")]
        public string[] MajorTypeHints { get; set; } = System.Array.Empty<string>();

        /// <summary>
        /// Determines commit's scopes how will create a major increment.
        /// </summary>
        [Option(ShortName = "hac", Description = "Determines commit's scopes how will create a major increment.")]
        public string[] MajorScopeHints { get; set; } = System.Array.Empty<string>();

        /// <summary>
        /// Determines commit's subject's hints how will create a major increment.
        /// </summary>
        [Option(ShortName = "hau", Description = "Determines commit's subject's hints how will create a major increment.")]
        public string[] MajorSubjectHints { get; set; } = { "#breaking" };

        /// <summary>
        /// Determines commit's types how will create a minor increment.
        /// </summary>
        [Option(ShortName = "hit", Description = "Determines commit's types how will create a minor increment.")]
        public string[] MinorTypeHints { get; set; } = System.Array.Empty<string>();

        /// <summary>
        /// Determines commit's scopes how will create a minor increment.
        /// </summary>
        [Option(ShortName = "hic", Description = "Determines commit's scopes how will create a minor increment.")]
        public string[] MinorScopeHints { get; set; } = System.Array.Empty<string>();

        /// <summary>
        /// Determines commit's subject's hints how will create a minor increment.
        /// </summary>
        [Option(ShortName = "hiu", Description = "Determines commit's subject's hints how will create a minor increment.")]
        public string[] MinorSubjectHints { get; set; } = System.Array.Empty<string>();

        /// <summary>
        /// Determines commit's types how will create a patch increment.
        /// </summary>
        [Option(ShortName = "hpt", Description = "Determines commit's types how will create a patch increment.")]
        public string[] PatchTypeHints { get; set; } = System.Array.Empty<string>();

        /// <summary>
        /// Determines commit's scopes how will create a patch increment.
        /// </summary>
        [Option(ShortName = "hpc", Description = "Determines commit's scopes how will create a patch increment.")]
        public string[] PatchScopeHints { get; set; } = System.Array.Empty<string>();

        /// <summary>
        /// Determines commit's subject's hints how will create a patch increment.
        /// </summary>
        [Option(ShortName = "hpu", Description = "Determines commit's subject's hints how will create a patch increment.")]
        public string[] PatchSubjectHints { get; set; } = System.Array.Empty<string>();

        // ReSharper disable once UnusedMember.Local
        private async Task OnExecuteAsync()
        {
            var command = new IncrementVersionWithCustomGitHintsCommand
            {
                GitDirectory = GitDirectory,
                TargetDirectory = TargetDirectory,
                SearchOption = SearchOption,
                CommitAuthorEmail = AuthorEmail,
                RemoteTarget = RemoteTarget,
                BranchName = BranchName,
                ExitBeta = ExitBeta,
                TagPrefix = TagPrefix,
                TagSuffix = TagSuffix,
                CaseInsensitiveHints = CaseInsensitiveHints,
                SkipTypeHints = new List<string>(SkipTypeHints),
                SkipScopeHints = new List<string>(SkipScopeHints),
                SkipSubjectHints = new List<string>(SkipSubjectHints),
                MajorTypeHints = new List<string>(MajorTypeHints),
                MajorScopeHints = new List<string>(MajorScopeHints),
                MajorSubjectHints = new List<string>(MajorSubjectHints),
                MinorTypeHints = new List<string>(MinorTypeHints),
                MinorScopeHints = new List<string>(MinorScopeHints),
                MinorSubjectHints = new List<string>(MinorSubjectHints),
                PatchTypeHints = new List<string>(PatchTypeHints),
                PatchScopeHints = new List<string>(PatchScopeHints),
                PatchSubjectHints = new List<string>(PatchSubjectHints)
            };
            await _mediator.Send(command, CancellationToken.None);
        }
    }
}
