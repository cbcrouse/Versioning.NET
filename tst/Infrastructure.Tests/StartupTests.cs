using Application.Interfaces;
using Infrastructure.Services;
using Infrastructure.Startup;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Xunit;

namespace Infrastructure.Tests;

public class StartupTests
{
    private IServiceProvider ServiceProvider { get; }

    public StartupTests()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddLogging();
        var configurationBuilder = new ConfigurationBuilder();
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");
        var configuration = configurationBuilder.Build();
        var orchestrator = new AppStartupOrchestrator();
        orchestrator.InitializeConfiguration(configuration);
        orchestrator.InitializeServiceCollection(serviceCollection);
        orchestrator.Orchestrate();

        ServiceProvider = serviceCollection.BuildServiceProvider();
    }

    [Fact]
    public void AssemblyVersioningService_IsAssignableFrom_IAssemblyVersioningService()
    {
        var sut = new AssemblyVersioningService();

        Assert.IsAssignableFrom<IAssemblyVersioningService>(sut);
    }

    [Fact]
    public void IAssemblyVersioningService_IsRegisteredCorrectly()
    {
        var sut = ServiceProvider.GetRequiredService<IAssemblyVersioningService>();

        Assert.IsType<AssemblyVersioningService>(sut);
    }

    [Fact]
    public void LibGit2Service_IsAssignableFrom_IGitService()
    {
        var sut = new LibGit2Service();

        Assert.IsAssignableFrom<IGitService>(sut);
    }

    [Fact]
    public void IGitService_IsRegisteredCorrectly()
    {
        var sut = ServiceProvider.GetRequiredService<IGitService>();

        Assert.IsType<LibGit2Service>(sut);
    }

    [Fact]
    public void GitVersioningService_IsAssignableFrom_IGitVersioningService()
    {
        var sut = new GitVersioningService();

        Assert.IsAssignableFrom<IGitVersioningService>(sut);
    }

    [Fact]
    public void IGitVersioningService_IsRegisteredCorrectly()
    {
        var sut = ServiceProvider.GetRequiredService<IGitVersioningService>();

        Assert.IsType<GitVersioningService>(sut);
    }
}