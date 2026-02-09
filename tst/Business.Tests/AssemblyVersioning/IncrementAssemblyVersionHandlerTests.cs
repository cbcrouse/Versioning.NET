using Application.AssemblyVersioning.Commands;
using Application.AssemblyVersioning.Handlers;
using Application.Interfaces;
using Domain.Enumerations;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Semver;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Business.Tests.AssemblyVersioning
{
    public class IncrementAssemblyVersionHandlerTests
    {
        [Fact]
        public async Task Handler_CallsDependencies()
        {
            // Arrange
            var request = new IncrementAssemblyVersionCommand
            {
                Directory = "C:\\Temp",
                VersionIncrement = VersionIncrement.Minor
            };
            var service = new Mock<IAssemblyVersioningService>();
            var logger = new NullLogger<IncrementAssemblyVersionHandler>();
            var sut = new IncrementAssemblyVersionHandler(service.Object, logger);

            // Act
            await sut.Handle(request, CancellationToken.None);

            // Assert
            service.Verify(x => x.IncrementVersion(It.IsAny<VersionIncrement>(), It.IsAny<string>(), It.IsAny<SearchOption>()), Times.Once);
        }

        [Fact]
        public async Task BetaVersion_LoweredToMinor_FromMajor()
        {
            // Arrange
            var request = new IncrementAssemblyVersionCommand
            {
                Directory = "C:\\Temp",
                VersionIncrement = VersionIncrement.Major
            };
            var service = new Mock<IAssemblyVersioningService>();
            service.Setup(x => x.GetLatestAssemblyVersion(It.IsAny<string>(), It.IsAny<SearchOption>())).Returns(new SemVersion(0, 1, 1));
            var logger = new NullLogger<IncrementAssemblyVersionHandler>();
            var sut = new IncrementAssemblyVersionHandler(service.Object, logger);

            // Act
            await sut.Handle(request, CancellationToken.None);

            // Assert
            service.Verify(x => x.IncrementVersion(VersionIncrement.Minor, It.IsAny<string>(), It.IsAny<SearchOption>()), Times.Once);
        }

        [Fact]
        public async Task BetaVersion_StaysMinor_WhenIncrementIsMinor()
        {
            // Arrange
            var request = new IncrementAssemblyVersionCommand
            {
                Directory = "C:\\Temp",
                VersionIncrement = VersionIncrement.Minor
            };
            var service = new Mock<IAssemblyVersioningService>();
            service.Setup(x => x.GetLatestAssemblyVersion(It.IsAny<string>(), It.IsAny<SearchOption>())).Returns(new SemVersion(0, 1, 1));
            var logger = new NullLogger<IncrementAssemblyVersionHandler>();
            var sut = new IncrementAssemblyVersionHandler(service.Object, logger);

            // Act
            await sut.Handle(request, CancellationToken.None);

            // Assert
            service.Verify(x => x.IncrementVersion(VersionIncrement.Minor, It.IsAny<string>(), It.IsAny<SearchOption>()), Times.Once);
        }

        [Fact]
        public async Task BetaVersion_StaysPatch_WhenIncrementIsPatch()
        {
            // Arrange
            var request = new IncrementAssemblyVersionCommand
            {
                Directory = "C:\\Temp",
                VersionIncrement = VersionIncrement.Patch
            };
            var service = new Mock<IAssemblyVersioningService>();
            service.Setup(x => x.GetLatestAssemblyVersion(It.IsAny<string>(), It.IsAny<SearchOption>())).Returns(new SemVersion(0, 1, 1));
            var logger = new NullLogger<IncrementAssemblyVersionHandler>();
            var sut = new IncrementAssemblyVersionHandler(service.Object, logger);

            // Act
            await sut.Handle(request, CancellationToken.None);

            // Assert
            service.Verify(x => x.IncrementVersion(VersionIncrement.Patch, It.IsAny<string>(), It.IsAny<SearchOption>()), Times.Once);
        }

        [Fact]
        public async Task BetaVersion_SetsIncrementToNone_WhenIncrementIsUnknown()
        {
            // Arrange
            var request = new IncrementAssemblyVersionCommand
            {
                Directory = "C:\\Temp",
                VersionIncrement = VersionIncrement.Unknown
            };
            var service = new Mock<IAssemblyVersioningService>();
            service.Setup(x => x.GetLatestAssemblyVersion(It.IsAny<string>(), It.IsAny<SearchOption>())).Returns(new SemVersion(0, 1, 1));
            var logger = new NullLogger<IncrementAssemblyVersionHandler>();
            var sut = new IncrementAssemblyVersionHandler(service.Object, logger);

            // Act
            await sut.Handle(request, CancellationToken.None);

            // Assert
            service.Verify(x => x.IncrementVersion(VersionIncrement.None, It.IsAny<string>(), It.IsAny<SearchOption>()), Times.Once);
        }

        [Fact]
        public async Task BetaVersion_Throws_WhenInvalidIncrement()
        {
            // Arrange
            var request = new IncrementAssemblyVersionCommand
            {
                Directory = "C:\\Temp",
                VersionIncrement = (VersionIncrement)6
            };
            var service = new Mock<IAssemblyVersioningService>();
            service.Setup(x => x.GetLatestAssemblyVersion(It.IsAny<string>(), It.IsAny<SearchOption>())).Returns(new SemVersion(0, 1, 1));
            var logger = new NullLogger<IncrementAssemblyVersionHandler>();
            var sut = new IncrementAssemblyVersionHandler(service.Object, logger);

            // Act
            var ex = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await sut.Handle(request, CancellationToken.None));

            // Assert
            Assert.Contains("Actual value was 6.", ex.Message);
        }

        [Fact]
        public async Task BetaVersion_DoesNotLowerIncrement_WhenExitBeta_IsTrue()
        {
            // Arrange
            var request = new IncrementAssemblyVersionCommand
            {
                Directory = "C:\\Temp",
                VersionIncrement = VersionIncrement.Major,
                ExitBeta = true
            };
            var service = new Mock<IAssemblyVersioningService>();
            service.Setup(x => x.GetLatestAssemblyVersion(It.IsAny<string>(), It.IsAny<SearchOption>())).Returns(new SemVersion(0, 1, 1));
            var logger = new NullLogger<IncrementAssemblyVersionHandler>();
            var sut = new IncrementAssemblyVersionHandler(service.Object, logger);

            // Act
            _ = await sut.Handle(request, CancellationToken.None);

            // Assert
            service.Verify(x => x.IncrementVersion(VersionIncrement.Major, It.IsAny<string>(), It.IsAny<SearchOption>()), Times.Once);
        }

        [Fact]
        public async Task BetaVersion_SetsIncrementToMajor_WhenExitBeta_IsTrue()
        {
            // Arrange
            var request = new IncrementAssemblyVersionCommand
            {
                Directory = "C:\\Temp",
                VersionIncrement = VersionIncrement.Minor,
                ExitBeta = true
            };
            var service = new Mock<IAssemblyVersioningService>();
            service.Setup(x => x.GetLatestAssemblyVersion(It.IsAny<string>(), It.IsAny<SearchOption>())).Returns(new SemVersion(0, 1, 1));
            var logger = new NullLogger<IncrementAssemblyVersionHandler>();
            var sut = new IncrementAssemblyVersionHandler(service.Object, logger);

            // Act
            _ = await sut.Handle(request, CancellationToken.None);

            // Assert
            service.Verify(x => x.IncrementVersion(VersionIncrement.Major, It.IsAny<string>(), It.IsAny<SearchOption>()), Times.Once);
        }

        [Fact]
        public async Task NonBetaVersion_Unchanged_WhenExitBeta_IsTrue()
        {
            // Arrange
            var request = new IncrementAssemblyVersionCommand
            {
                Directory = "C:\\Temp",
                VersionIncrement = VersionIncrement.Minor,
                ExitBeta = true
            };
            var service = new Mock<IAssemblyVersioningService>();
            service.Setup(x => x.GetLatestAssemblyVersion(It.IsAny<string>(), It.IsAny<SearchOption>())).Returns(new SemVersion(1, 1, 1));
            var logger = new NullLogger<IncrementAssemblyVersionHandler>();
            var sut = new IncrementAssemblyVersionHandler(service.Object, logger);

            // Act
            _ = await sut.Handle(request, CancellationToken.None);

            // Assert
            service.Verify(x => x.IncrementVersion(VersionIncrement.Minor, It.IsAny<string>(), It.IsAny<SearchOption>()), Times.Once);
        }
    }
}
