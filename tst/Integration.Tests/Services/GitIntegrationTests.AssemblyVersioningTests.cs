using Application.Interfaces;
using Domain.Enumerations;
using Infrastructure.Services;
using Semver;
using System;
using Xunit;

namespace Integration.Tests.Services
{
    public partial class GitIntegrationTests
    {
        [Fact]
        public void AssemblyVersioningService_Implements_IAssemblyVersioningService()
        {
            // Arrange & Act
            var service = typeof(AssemblyVersioningService).GetInterface(nameof(IAssemblyVersioningService));

            // Assert
            Assert.NotNull(service);
        }

        [Fact]
        public void Can_GetLatestAssemblyVersion()
        {
            // Arrange
            var service = new AssemblyVersioningService();

            // Act
            SemVersion result = service.GetLatestAssemblyVersion(TestRepoDirectory);

            // Assert
            Assert.True(result != default);
        }

        [Fact]
        public void Can_IncrementAssemblyVersion_By_Patch()
        {
            // Arrange
            var service = new AssemblyVersioningService();
            SemVersion originalAssemblyVersion = service.GetLatestAssemblyVersion(TestRepoDirectory);
            SemVersion expectedAssemblyVersion = new SemVersion(originalAssemblyVersion.Major, originalAssemblyVersion.Minor, originalAssemblyVersion.Patch+1);

            // Act
            service.IncrementVersion(VersionIncrement.Patch, TestRepoDirectory);

            // Assert
            SemVersion actualAssemblyVersion = service.GetLatestAssemblyVersion(TestRepoDirectory);
            Assert.Equal(expectedAssemblyVersion, actualAssemblyVersion);
        }

        [Fact]
        public void Can_IncrementAssemblyVersion_By_Minor()
        {
            // Arrange
            var service = new AssemblyVersioningService();
            SemVersion originalAssemblyVersion = service.GetLatestAssemblyVersion(TestRepoDirectory);
            SemVersion expectedAssemblyVersion = new SemVersion(originalAssemblyVersion.Major, originalAssemblyVersion.Minor+1);

            // Act
            service.IncrementVersion(VersionIncrement.Minor, TestRepoDirectory);

            // Assert
            SemVersion actualAssemblyVersion = service.GetLatestAssemblyVersion(TestRepoDirectory);
            Assert.Equal(expectedAssemblyVersion, actualAssemblyVersion);
        }

        [Fact]
        public void Can_IncrementAssemblyVersion_By_Major()
        {
            // Arrange
            var service = new AssemblyVersioningService();
            SemVersion originalAssemblyVersion = service.GetLatestAssemblyVersion(TestRepoDirectory);
            SemVersion expectedAssemblyVersion = new SemVersion(originalAssemblyVersion.Major+1);

            // Act
            service.IncrementVersion(VersionIncrement.Major, TestRepoDirectory);

            // Assert
            SemVersion actualAssemblyVersion = service.GetLatestAssemblyVersion(TestRepoDirectory);
            Assert.Equal(expectedAssemblyVersion, actualAssemblyVersion);
        }

        [Fact]
        public void Can_IncrementAssemblyVersion_By_None()
        {
            // Arrange
            var service = new AssemblyVersioningService();
            SemVersion originalAssemblyVersion = service.GetLatestAssemblyVersion(TestRepoDirectory);

            // Act
            service.IncrementVersion(VersionIncrement.None, TestRepoDirectory);

            // Assert
            SemVersion actualAssemblyVersion = service.GetLatestAssemblyVersion(TestRepoDirectory);
            Assert.Equal(originalAssemblyVersion, actualAssemblyVersion);
        }

        [Fact]
        public void IncrementAssemblyVersion_Throws_WhenInvalidIncrement()
        {
            // Arrange
            var service = new AssemblyVersioningService();

            // Act
            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => service.IncrementVersion((VersionIncrement)6, TestRepoDirectory));

            // Assert
            Assert.Contains("Exception of type 'System.ArgumentOutOfRangeException' was thrown.", ex.Message);
            Assert.Contains("Actual value was 6.", ex.Message);
        }
    }
}
