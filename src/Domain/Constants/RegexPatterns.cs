namespace Domain.Constants
{
    /// <summary>
    /// Various regex pattern constants.
    /// </summary>
    public static class RegexPatterns
    {
        /// <summary>
        /// Matches the commit ID at the start in a git log line.
        /// </summary>
        public const string GitLogCommitId = "(?<CommitId>[A-Za-z0-9]{7,}) ";

        /// <summary>
        /// Matches the commit message subject in a git log line.
        /// </summary>
        public const string GitLogCommitSubject = @"(?<Type>\w{1,})\((?<Scope>[\w\s]{1,})\)\s*:\s*(?<Subject>\w{1,})";
    }
}
