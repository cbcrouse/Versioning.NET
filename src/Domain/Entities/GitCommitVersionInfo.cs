﻿using Domain.Enumerations;

namespace Domain.Entities
{
    /// <summary>
    /// Provides a structure to represent Angular's commit message standard.
    /// <para>https://github.com/angular/angular/blob/master/CONTRIBUTING.md#commit</para>
    /// </summary>
    public class GitCommitVersionInfo
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="type">The 'type' from the commit message.</param>
        /// <param name="scope">The 'scope' from the commit message.</param>
        /// <param name="subject">The 'subject' from the commit message.</param>
        public GitCommitVersionInfo(string type, string scope, string subject)
        {
            Type = type;
            Scope = scope;
            Subject = subject;
            ExitBeta = DetermineBetaExit(subject);
            VersionIncrement = GetVersionIncrement(subject, type);
        }

        /// <summary>
        /// Describes the type of the commit.
        /// </summary>
        public string Type { get; }

        /// <summary>
        /// Describes the scope of the commit.
        /// </summary>
        public string Scope { get; }

        /// <summary>
        /// Describes subject of the commit.
        /// </summary>
        public string Subject { get; }

        /// <summary>
        /// Determines whether the commit signifies beta mode should be exited.
        /// </summary>
        public bool ExitBeta { get; }

        /// <summary>
        /// Indicates the version increment of the commit.
        /// </summary>
        public VersionIncrement VersionIncrement { get; }

        private static bool DetermineBetaExit(string subject)
        {
            return subject.ToLower().Contains("[exit beta]");
        }

        /// <summary>
        /// Calculates the version increment from the git commit message.
        /// </summary>
        private static VersionIncrement GetVersionIncrement(string subject, string type)
        {
            string lowerCaseSubject = subject.ToLower();

            if (lowerCaseSubject.Contains("[skip hint]"))
            {
                return VersionIncrement.None;
            }

            if (lowerCaseSubject.Contains("#breaking"))
            {
                return VersionIncrement.Major;
            }

            return type.ToLower() switch
            {
                "feat" => VersionIncrement.Minor,
                "fix" => VersionIncrement.Patch,
                "build" => VersionIncrement.Patch,
                "config" => VersionIncrement.Patch,
                "docs" => VersionIncrement.Patch,
                "perf" => VersionIncrement.Patch,
                "refactor" => VersionIncrement.Patch,
                "resolve" => VersionIncrement.Patch,
                "style" => VersionIncrement.Patch,
                "test" => VersionIncrement.Patch,
                "ci" => VersionIncrement.Patch,
                _ => VersionIncrement.Unknown
            };
        }
    }
}
