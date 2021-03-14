using Common.Configuration;
using Infrastructure.Startup;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace Integration.Tests.Setup
{
    public class Orchestrator
    {
        public IServiceProvider BuildServiceProvider()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging(builder => builder.AddConsole());
            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddPrioritizedSettings();
            var configuration = configurationBuilder.Build();
            var orchestrator = new AppStartupOrchestrator();
            orchestrator.InitializeConfiguration(configuration);
            orchestrator.InitializeServiceCollection(serviceCollection);
            orchestrator.Orchestrate();
            return serviceCollection.BuildServiceProvider();
        }
    }
}
