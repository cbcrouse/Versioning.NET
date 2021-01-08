using Application.Interfaces;
using Domain.Enumerations;
using Infrastructure.Services;
using Infrastructure.Tests.Setup;
using Semver;
using Xunit;

namespace Infrastructure.Tests.Services
{
    public class AssemblyVersioningServiceTests : GitSetup
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
        public void Can_IncrementAssemblyVersion()
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
    }
}
