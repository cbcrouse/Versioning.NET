﻿using Application.Interfaces;
using Infrastructure.Services;
using Infrastructure.Startup;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Xunit;

namespace Infrastructure.Tests
{
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
            // Arrange
            var sut = new AssemblyVersioningService();

            // Act & Assert
            Assert.IsAssignableFrom<IAssemblyVersioningService>(sut);

        }

        [Fact]
        public void IAssemblyVersioningService_IsRegisteredCorrectly()
        {
            // Arrange
            var sut = ServiceProvider.GetRequiredService<IAssemblyVersioningService>();

            // Act & Assert
            Assert.IsType<AssemblyVersioningService>(sut);

        }

        [Fact]
        public void LibGit2Service_IsAssignableFrom_IGitService()
        {
            // Arrange
            var sut = new LibGit2Service();

            // Act & Assert
            Assert.IsAssignableFrom<IGitService>(sut);

        }

        [Fact]
        public void IGitService_IsRegisteredCorrectly()
        {
            // Arrange
            var sut = ServiceProvider.GetRequiredService<IGitService>();

            // Act & Assert
            Assert.IsType<LibGit2Service>(sut);

        }

        [Fact]
        public void GitVersioningService_IsAssignableFrom_IGitVersioningService()
        {
            // Arrange
            var sut = new GitVersioningService();

            // Act & Assert
            Assert.IsAssignableFrom<IGitVersioningService>(sut);

        }

        [Fact]
        public void IGitVersioningService_IsRegisteredCorrectly()
        {
            // Arrange
            var sut = ServiceProvider.GetRequiredService<IGitVersioningService>();

            // Act & Assert
            Assert.IsType<GitVersioningService>(sut);

        }
    }
}
