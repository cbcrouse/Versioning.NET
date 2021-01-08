using Application.Interfaces;
using Domain.Enumerations;
using Infrastructure.Services;
using Semver;
using Xunit;

namespace Infrastructure.Tests.Services
{
    public class AssemblyVersioningServiceTests
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
            SemVersion result = service.GetLatestAssemblyVersion("G:\\Git\\CleanArchitecture");

            // Assert
            Assert.True(result != default);
        }

        [Fact]
        public void Can_IncrementAssemblyVersion()
        {
            // Arrange
            var service = new AssemblyVersioningService();
            SemVersion originalAssemblyVersion = service.GetLatestAssemblyVersion("G:\\Git\\CleanArchitecture");
            SemVersion expectedAssemblyVersion = new SemVersion(originalAssemblyVersion.Major, originalAssemblyVersion.Minor, originalAssemblyVersion.Patch+1);

            // Act
            service.IncrementVersion(VersionIncrement.Patch, "G:\\Git\\CleanArchitecture");

            // Assert
            SemVersion actualAssemblyVersion = service.GetLatestAssemblyVersion("G:\\Git\\CleanArchitecture");
            Assert.Equal(expectedAssemblyVersion, actualAssemblyVersion);
        }
    }
}
