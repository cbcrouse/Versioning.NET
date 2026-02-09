#nullable enable
using System.IO;
using System.Linq;

namespace Domain.Entities;

/// <summary>
/// Represents a file associated to a git commit record.
/// </summary>
public class GitCommitFileInfo
{
    /// <summary>
    /// Default Constructor
    /// </summary>
    /// <param name="gitDirectoryPath">The directory containing the .git folder.</param>
    /// <param name="gitCommitLogFilePath">The file path from a git commit log output.</param>
    public GitCommitFileInfo(string gitDirectoryPath, string gitCommitLogFilePath)
    {
        var originalFile = new FileInfo(Path.Join(gitDirectoryPath, gitCommitLogFilePath));
        CurrentFilePath = null;

        if (!originalFile.Exists)
        {
            string fileName = Path.GetFileName(gitCommitLogFilePath);
            CurrentFilePath = Directory.GetFiles(gitDirectoryPath, fileName, SearchOption.AllDirectories).FirstOrDefault();
        }

        FileMoved = !originalFile.Exists && CurrentFilePath != null;
        FileExists = originalFile.Exists || CurrentFilePath != null;
        OriginalFilePath = originalFile.Exists ? originalFile.FullName : gitCommitLogFilePath;
        CurrentFilePath ??= originalFile.Exists ? OriginalFilePath : null;
        FileRemoved = !FileExists && CurrentFilePath == null;
    }

    /// <summary>
    /// Indicates that the file still exists.
    /// </summary>
    public bool FileExists { get; }

    /// <summary>
    /// Indicates that the file has been moved since this commit was created.
    /// </summary>
    public bool FileMoved { get; }

    /// <summary>
    /// Indicates that the file has been removed since this commit was created.
    /// </summary>
    public bool FileRemoved { get; }

    /// <summary>
    /// The file path from the git commit.
    /// </summary>
    public string OriginalFilePath { get; }

    /// <summary>
    /// The current location of the file.
    /// </summary>
    public string? CurrentFilePath { get; }
}