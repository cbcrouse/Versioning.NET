using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain.Entities;

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
    /// <param name="updates">The file updates associated to the commit.</param>
    public GitCommit(string id, string subject, IEnumerable<GitCommitFileInfo> updates)
    {
        if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException(id);
        if (string.IsNullOrWhiteSpace(subject)) throw new ArgumentException(subject);

        Id = id;
        Subject = subject;
        Updates = updates.ToList();
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
    /// A collection of associated file updates for the commit.
    /// </summary>
    public List<GitCommitFileInfo> Updates { get; }
}