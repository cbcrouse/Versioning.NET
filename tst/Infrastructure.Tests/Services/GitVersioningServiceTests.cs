using Domain.Entities;
using Domain.Enumerations;
using Infrastructure.Services;
using Semver;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Xunit;

namespace Infrastructure.Tests.Services
{
    public class GitVersioningServiceTests
    {
        public static IEnumerable<object[]> VersionData
        {
            get
            {
                return new[]
                {
                    new object[] { new[] {"v2.2.4", "v2.2.5", "v2.2.6" }, "2.2.6" }, // Patch
                    new object[] { new[] {"v2.2.4", "v2.2.5", "v2.3.0" }, "2.3.0" }, // Minor
                    new object[] { new[] {"v2.2.4", "v2.2.5", "v2.2.6" }, "2.2.6" }, // Major
                    new object[] { new[] {"v2.4.0-alpha", "v2.4.0-beta", "v2.4.0-rc" }, "2.4.0-rc" }, // PreRelease
                    new object[] { new[] {"v2.4.0-beta.81", "v2.4.0-beta.82", "v2.4.0-beta.99" }, "2.4.0-beta.99" }, // Build
                    new object[] { new[] {"v2.4.0-alpha.70", "v2.4.0-beta.81", "v2.4.0-beta.99", "v2.4.0-rc.103" }, "2.4.0-rc.103" }, // PreRelease + Build
                    new object[] { new[] {"v3.0.0-alpha.268", "v3.0.0-rc.289", "v3.0.0" }, "3.0.0" }, // PreRelease + MajorRelease
                };
            }
        }

        [Theory]
        [MemberData(nameof(VersionData))]
        public void CanGetLatestVersionTag(IEnumerable<string> tags, string expectedVersion)
        {
            // Arrange
            var sut = new GitVersioningService();
            SemVersion.TryParse(expectedVersion, out SemVersion expectedSemVersion);

            // Act
            KeyValuePair<string, SemVersion> latestVersionTag = sut.GetLatestVersionTag(tags);

            // Assert
            Assert.Equal(expectedSemVersion, latestVersionTag.Value);
        }

        [Theory]
        [InlineData("build(Pipeline): Downgraded to .net core 3.1 from 5 [skip ci]", "build", "Pipeline", "Downgraded to .net core 3.1 from 5 [skip ci]", VersionIncrement.Patch)]
        [InlineData("feat(Tools): Updated test script to clean out .templateengine directory", "feat", "Tools", "Updated test script to clean out .templateengine directory", VersionIncrement.Minor)]
        [InlineData("feat(Template): Added ability to see template in Visual Studio", "feat", "Template", "Added ability to see template in Visual Studio", VersionIncrement.Minor)]
        [InlineData("config(Projects): Loaded unloaded projects [skip ci]", "config", "Projects", "Loaded unloaded projects [skip ci]", VersionIncrement.Patch)]
        [InlineData("docs(README): Added \"Startup Orchestration\" docs and Solution Overview [skip ci]", "docs", "README", "Added \"Startup Orchestration\" docs and Solution Overview [skip ci]", VersionIncrement.Patch)]
        [InlineData("ci(Build): Updated build.yml to target the template solution file", "ci", "Build", "Updated build.yml to target the template solution file", VersionIncrement.Patch)]
        [InlineData("fake(Build): Updated build.yml to target the template solution file", "fake", "Build", "Updated build.yml to target the template solution file", VersionIncrement.Unknown)]
        [InlineData("fake(Build): Updated build.yml to target the template solution file [skip hint]", "fake", "Build", "Updated build.yml to target the template solution file [skip hint]", VersionIncrement.None)]
        [InlineData("perf(API): Decreased response time to <200ms", "perf", "API", "Decreased response time to <200ms", VersionIncrement.Patch)]
        [InlineData("refactor(Application): Removed duplicate code", "refactor", "Application", "Removed duplicate code", VersionIncrement.Patch)]
        [InlineData("resolve(Persistence): Resolved a merge conflict", "resolve", "Persistence", "Resolved a merge conflict", VersionIncrement.Patch)]
        [InlineData("test(Infrastructure): Added additional mapping tests", "test", "Infrastructure", "Added additional mapping tests", VersionIncrement.Patch)]
        public void CanMatchSubjectLine(string input, string expectedType, string expectedScope, string expectedSubject, VersionIncrement expectedIncrement)
        {
            // Arrange
            var sut = new GitVersioningService();

            // Act
            GitCommitVersionInfo info = sut.GetCommitVersionInfo(new List<string>{ input }).First();

            // Assert
            Assert.Equal(expectedType, info.Type);
            Assert.Equal(expectedScope, info.Scope);
            Assert.Equal(expectedSubject, info.Subject);
            Assert.Equal(expectedIncrement, info.VersionIncrement);
        }

        [Fact]
        public void CanGetVersionInfoFromCommitSubjectLines()
        {
            // Arrange
            var sut = new GitVersioningService();
            string executableLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string fileLocation = Path.Combine(executableLocation ?? string.Empty, "CommitMessageSamples.txt");
            string[] messages = File.ReadAllLines(fileLocation);
            int expectedPatchCount = 7;
            int expectedMinorCount = 4;
            int expectedMajorCount = 1;
            int expectedNoneCount = 1;

            // Act
            IEnumerable<GitCommitVersionInfo> results = sut.GetCommitVersionInfo(messages);
            IEnumerable<GitCommitVersionInfo> gitCommitVersionInfos = results.ToList();
            int patchCount = gitCommitVersionInfos.Count(x => x.VersionIncrement == VersionIncrement.Patch);
            int minorCount = gitCommitVersionInfos.Count(x => x.VersionIncrement == VersionIncrement.Minor);
            int majorCount = gitCommitVersionInfos.Count(x => x.VersionIncrement == VersionIncrement.Major);
            int noneCount = gitCommitVersionInfos.Count(x => x.VersionIncrement == VersionIncrement.None);

            // Assert
            Assert.Equal(expectedPatchCount, patchCount);
            Assert.Equal(expectedMinorCount, minorCount);
            Assert.Equal(expectedMajorCount, majorCount);
            Assert.Equal(expectedNoneCount, noneCount);
        }

        [Theory]
        [InlineData(new[] { VersionIncrement.Major, VersionIncrement.Minor, VersionIncrement.Patch, VersionIncrement.None, VersionIncrement.Unknown }, VersionIncrement.Major)]
        [InlineData(new[] { VersionIncrement.Minor, VersionIncrement.Patch, VersionIncrement.None, VersionIncrement.Unknown }, VersionIncrement.Minor)]
        [InlineData(new[] { VersionIncrement.Patch, VersionIncrement.None, VersionIncrement.Unknown }, VersionIncrement.Patch)]
        [InlineData(new[] { VersionIncrement.None, VersionIncrement.Unknown }, VersionIncrement.None)]
        [InlineData(new[] { VersionIncrement.Unknown }, VersionIncrement.Unknown)]
        public void CanDetermineVersionIncrementPriority(IEnumerable<VersionIncrement> input, VersionIncrement expectedPriorityIncrement)
        {
            // Arrange
            var sut = new GitVersioningService();

            // Act
            VersionIncrement priorityIncrement = sut.DeterminePriorityIncrement(input);

            // Assert
            Assert.Equal(expectedPriorityIncrement, priorityIncrement);
        }
    }
}
