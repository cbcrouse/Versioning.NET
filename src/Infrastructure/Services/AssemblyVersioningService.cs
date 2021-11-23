#nullable enable
using Application.Interfaces;
using Domain.Enumerations;
using Semver;
using System;
using System.IO;
using System.Xml;

namespace Infrastructure.Services
{
    /// <summary>
    /// Provides the implementation details for working with assembly versions.
    /// </summary>
    public class AssemblyVersioningService : IAssemblyVersioningService
    {
        /// <summary>
        /// Returns the latest semantic version found amongst the files within the given directory.
        /// </summary>
        /// <param name="directory">The directory used to determine the latest version.</param>
        /// <param name="searchOption">Specifies whether to search the current directory, or the current directory and all subdirectories.</param>
        public SemVersion GetLatestAssemblyVersion(string directory, SearchOption searchOption)
        {
            string[] csProjFiles = Directory.GetFiles(directory, "*.csproj", searchOption);
            var highestVersion = new SemVersion(0,0,0);

            foreach (string csProjFile in csProjFiles)
            {
                var doc = new XmlDocument();
                doc.Load(csProjFile);
                string? versionText = doc.SelectSingleNode("/Project/PropertyGroup/VersionPrefix")?.InnerText ??
                                      doc.SelectSingleNode("/Project/PropertyGroup/Version")?.InnerText;

                if (versionText == null)
                {
                    continue;
                }

                SemVersion? version = SemVersion.Parse(versionText);

                if (version > highestVersion)
                    highestVersion = version;
            }

            return highestVersion;
        }

        /// <summary>
        /// Increases the assembly version by the specified <see cref="VersionIncrement"/>.
        /// </summary>
        /// <param name="increment">The increment to use when updating the version.</param>
        /// <param name="directory">The directory of the assemblies to version.</param>
        /// <param name="searchOption">Specifies whether to search the current directory, or the current directory and all subdirectories.</param>
        public void IncrementVersion(VersionIncrement increment, string directory, SearchOption searchOption)
        {
            string[] csProjFiles = Directory.GetFiles(directory, "*.csproj", searchOption);

            foreach (string csProjFile in csProjFiles)
            {
                var doc = new XmlDocument();
                doc.Load(csProjFile);

                XmlNode? versionPrefixText = doc.SelectSingleNode("/Project/PropertyGroup/VersionPrefix");
                XmlNode? versionText =  doc.SelectSingleNode("/Project/PropertyGroup/Version");

                if (versionPrefixText?.InnerText != null)
                {
                    UpdateVersionNode(increment, versionPrefixText);
                }
                else if (versionText?.InnerText != null)
                {
                    UpdateVersionNode(increment, versionText);
                }

                doc.Save(csProjFile);
            }
        }

        private static void UpdateVersionNode(VersionIncrement increment, XmlNode node)
        {
            SemVersion? version = SemVersion.Parse(node.InnerText);
            switch (increment)
            {
                case VersionIncrement.Unknown:
                case VersionIncrement.None:
                    break;
                case VersionIncrement.Patch:
                    version = new SemVersion(version.Major, version.Minor, version.Patch + 1, build: version.Build);
                    break;
                case VersionIncrement.Minor:
                    version = new SemVersion(version.Major, version.Minor + 1, 0);
                    break;
                case VersionIncrement.Major:
                    version = new SemVersion(version.Major + 1, 0, 0);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(increment), increment, null);
            }

            node.InnerText = version.ToString();
        }
    }
}
