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

namespace Business.Tests.AssemblyVersioning;

public class IncrementAssemblyVersionHandlerTests
{
    private static readonly string DirectoryPath = "C:\\Temp";

    private static (IncrementAssemblyVersionHandler Sut, Mock<IAssemblyVersioningService> Service) CreateSut(SemVersion latestVersion)
    {
        var service = new Mock<IAssemblyVersioningService>();
        service
            .Setup(x => x.GetLatestAssemblyVersion(It.IsAny<string>(), It.IsAny<SearchOption>()))
            .Returns(latestVersion);

        var logger = new NullLogger<IncrementAssemblyVersionHandler>();
        var sut = new IncrementAssemblyVersionHandler(service.Object, logger);

        return (sut, service);
    }

    private static IncrementAssemblyVersionCommand CreateRequest(VersionIncrement increment, bool exitBeta = false)
        => new()
        {
            Directory = DirectoryPath,
            VersionIncrement = increment,
            ExitBeta = exitBeta
        };

    [Fact]
    public async Task Handler_CallsDependencies()
    {
        var request = CreateRequest(VersionIncrement.Minor);
        var (sut, service) = CreateSut(new SemVersion(0, 1, 1));

        await sut.Handle(request, CancellationToken.None);

        service.Verify(versioningService => versioningService
                .IncrementVersion(It.IsAny<VersionIncrement>(), DirectoryPath, It.IsAny<SearchOption>()), Times.Once);
    }

    [Fact]
    public async Task BetaVersion_LoweredToMinor_FromMajor()
    {
        var request = CreateRequest(VersionIncrement.Major);
        var (sut, service) = CreateSut(new SemVersion(0, 1, 1));

        await sut.Handle(request, CancellationToken.None);

        service.Verify(versioningService => versioningService
                .IncrementVersion(VersionIncrement.Minor, DirectoryPath, It.IsAny<SearchOption>()), Times.Once);
    }

    [Fact]
    public async Task BetaVersion_StaysMinor_WhenIncrementIsMinor()
    {
        var request = CreateRequest(VersionIncrement.Minor);
        var (sut, service) = CreateSut(new SemVersion(0, 1, 1));

        await sut.Handle(request, CancellationToken.None);

        service.Verify(versioningService => versioningService
                .IncrementVersion(VersionIncrement.Minor, DirectoryPath, It.IsAny<SearchOption>()), Times.Once);
    }

    [Fact]
    public async Task BetaVersion_StaysPatch_WhenIncrementIsPatch()
    {
        var request = CreateRequest(VersionIncrement.Patch);
        var (sut, service) = CreateSut(new SemVersion(0, 1, 1));

        await sut.Handle(request, CancellationToken.None);

        service.Verify(versioningService => versioningService
                .IncrementVersion(VersionIncrement.Patch, DirectoryPath, It.IsAny<SearchOption>()), Times.Once);
    }

    [Fact]
    public async Task BetaVersion_SetsIncrementToNone_WhenIncrementIsUnknown()
    {
        var request = CreateRequest(VersionIncrement.Unknown);
        var (sut, service) = CreateSut(new SemVersion(0, 1, 1));

        await sut.Handle(request, CancellationToken.None);

        service.Verify(versioningService => versioningService
                .IncrementVersion(VersionIncrement.None, DirectoryPath, It.IsAny<SearchOption>()), Times.Once);
    }

    [Fact]
    public async Task BetaVersion_Throws_WhenInvalidIncrement()
    {
        var request = CreateRequest((VersionIncrement)6);
        var (sut, _) = CreateSut(new SemVersion(0, 1, 1));

        var ex = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => sut.Handle(request, CancellationToken.None));

        Assert.Contains("Actual value was 6.", ex.Message);
    }

    [Fact]
    public async Task BetaVersion_DoesNotLowerIncrement_WhenExitBeta_IsTrue()
    {
        var request = CreateRequest(VersionIncrement.Major, exitBeta: true);
        var (sut, service) = CreateSut(new SemVersion(0, 1, 1));

        await sut.Handle(request, CancellationToken.None);

        service.Verify(versioningService => versioningService
                .IncrementVersion(VersionIncrement.Major, DirectoryPath, It.IsAny<SearchOption>()), Times.Once);
    }

    [Fact]
    public async Task BetaVersion_SetsIncrementToMajor_WhenExitBeta_IsTrue()
    {
        var request = CreateRequest(VersionIncrement.Minor, exitBeta: true);
        var (sut, service) = CreateSut(new SemVersion(0, 1, 1));

        await sut.Handle(request, CancellationToken.None);

        service.Verify(versioningService => versioningService
                .IncrementVersion(VersionIncrement.Major, DirectoryPath, It.IsAny<SearchOption>()), Times.Once);
    }

    [Fact]
    public async Task NonBetaVersion_Unchanged_WhenExitBeta_IsTrue()
    {
        var request = CreateRequest(VersionIncrement.Minor, exitBeta: true);
        var (sut, service) = CreateSut(new SemVersion(1, 1, 1));

        await sut.Handle(request, CancellationToken.None);

        service.Verify(versioningService => versioningService
                .IncrementVersion(VersionIncrement.Minor, DirectoryPath, It.IsAny<SearchOption>()), Times.Once);
    }
}
