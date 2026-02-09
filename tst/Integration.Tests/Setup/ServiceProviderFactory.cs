using Infrastructure.Startup;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace Integration.Tests.Setup;

public class ServiceProviderFactory
{
    public IServiceProvider Create()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddLogging(builder => builder.AddConsole());
        var configurationBuilder = new ConfigurationBuilder();
        var configuration = configurationBuilder.Build();
        var pipeline = new CoreServiceRegistrationPipeline();
        pipeline.Execute(serviceCollection, configuration);
        return serviceCollection.BuildServiceProvider();
    }
}