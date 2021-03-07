using System;

namespace Domain.Enumerations
{
    /// <summary>
    /// Provides several indicators for sorting git commits.
    /// </summary>
    [Flags]
    public enum GitCommitSortMode
    {
        /// <summary>
        /// Indicates the sort mode is none.
        /// </summary>
        None,
        /// <summary>
        /// Indicates the sort mode is descending.
        /// </summary>
        Descending
    }
}
