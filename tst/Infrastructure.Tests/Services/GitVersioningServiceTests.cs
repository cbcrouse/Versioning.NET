using Domain.Entities;
using Domain.Enumerations;
using Infrastructure.Services;
using Semver;
using System.Collections.Generic;
using System.Linq;
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
        [InlineData("2838s830", "build(Pipeline): Downgraded to .net core 3.1 from 5 [skip ci]", "build", "Pipeline", "Downgraded to .net core 3.1 from 5 [skip ci]", VersionIncrement.Patch)]
        [InlineData("2838s830", "feat(Tools): Updated test script to clean out .templateengine directory", "feat", "Tools", "Updated test script to clean out .templateengine directory", VersionIncrement.Minor)]
        [InlineData("2838s830", "feat(Template): Added ability to see template in Visual Studio", "feat", "Template", "Added ability to see template in Visual Studio", VersionIncrement.Minor)]
        [InlineData("2838s830", "config(Projects): Loaded unloaded projects [skip ci]", "config", "Projects", "Loaded unloaded projects [skip ci]", VersionIncrement.Patch)]
        [InlineData("2838s830", "docs(README): Added \"Startup Orchestration\" docs and Solution Overview [skip ci]", "docs", "README", "Added \"Startup Orchestration\" docs and Solution Overview [skip ci]", VersionIncrement.Patch)]
        [InlineData("2838s830", "ci(Build): Updated build.yml to target the template solution file", "ci", "Build", "Updated build.yml to target the template solution file", VersionIncrement.Patch)]
        [InlineData("2838s830", "fake(Build): Updated build.yml to target the template solution file", "fake", "Build", "Updated build.yml to target the template solution file", VersionIncrement.Unknown)]
        [InlineData("2838s830", "fake(Build): Updated build.yml to target the template solution file [skip hint]", "fake", "Build", "Updated build.yml to target the template solution file [skip hint]", VersionIncrement.None)]
        [InlineData("2838s830", "perf(API): Decreased response time to <200ms", "perf", "API", "Decreased response time to <200ms", VersionIncrement.Patch)]
        [InlineData("2838s830", "refactor(Application): Removed duplicate code", "refactor", "Application", "Removed duplicate code", VersionIncrement.Patch)]
        [InlineData("2838s830", "resolve(Persistence): Resolved a merge conflict", "resolve", "Persistence", "Resolved a merge conflict", VersionIncrement.Patch)]
        [InlineData("2838s830", "test(Infrastructure): Added additional mapping tests", "test", "Infrastructure", "Added additional mapping tests", VersionIncrement.Patch)]
        public void CanMatchSubjectLine(string id, string subject, string expectedType, string expectedScope, string expectedSubject, VersionIncrement expectedIncrement)
        {
            // Arrange
            var commit = new GitCommit(id, subject, new List<GitCommitFileInfo>());
            var sut = new GitVersioningService();

            // Act
            GitCommitVersionInfo info = sut.GetCommitVersionInfo(new List<GitCommit>{ commit }).First();

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
            var commits = new Dictionary<string, string>
            {
                {"54s6848", "fake(Build): Updated build.yml to target the template solution file [skip hint]"},
                {"27d8880", "build(Pipeline): Downgraded to .net core 3.1 from 5 [skip ci]"},
                {"9588148", "build(Actions): Updated dotnet version in PR Action [skip ci]"},
                {"adfe3d6", "docs(README): Updated Getting Started section [skip ci]"},
                {"65003d2", "feat(Tools): Updated test script to clean out .templateengine directory #breaking"},
                {"1889a29", "feat(Template): Added ability to see template in Visual Studio"},
                {"cef6fff", "update to use port gen. It will fallback to generated port if none provided"},
                {"21d4172", "Update to use sourceName instead of app-name and app-name-lower parameters"},
                {"82fb127", "Updates to make the template work in Visual Studio. This is a work in progress. We should replace the app name and port symbols to use the built-in support to provide a good experience. Since the app name param is not appearing, when run in VS the name given has to be DefaultAppName. After changes to use built-in support things will work as expected."},
                {"d2b8e8b", "updating package version to 1.3.1"},
                {"c41daee", "update to ps1 file to restore previous working directory"},
                {"d7df356", "feat(Template): Updated primaryOutput to list all projects instead of sln"},
                {"70a8f91", "feat(Template): Added ability to see template in Visual Studio"},
                {"07766a9", "docs(README): Updated azure pipeline status badge [skip ci]"},
                {"0dd8382", "feat(Docker): Added container orchestration support"},
                {"4db6707", "fix(Build): dotnet new & Package Downgrade Detected"},
                {"05a5def", "config(Projects): Loaded unloaded projects [skip ci]"},
                {"60c25be", "style(AppStartupOrchestrator): Removed extraneous TODO [skip ci]"}

            }.Select(x => new GitCommit(x.Key, x.Value, new List<GitCommitFileInfo>()));
            int expectedPatchCount = 7;
            int expectedMinorCount = 4;
            int expectedMajorCount = 1;
            int expectedNoneCount = 1;

            // Act
            IEnumerable<GitCommitVersionInfo> results = sut.GetCommitVersionInfo(commits);
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
