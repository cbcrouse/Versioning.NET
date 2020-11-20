using Application.Configuration;
using AutoMapper;
using AutoMapper.Configuration;
using Infrastructure.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Infrastructure.Startup
{
    /// <summary>
    /// Facilitates dependency registrations for the application.
    /// </summary>
    public class AppStartupOrchestrator : CoreStartupOrchestrator
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        public AppStartupOrchestrator()
        {
            ServiceRegistrationExpressions.Add(() => ServiceCollection.AddOptions());
            ServiceRegistrationExpressions.Add(() => ServiceCollection.AddTransient(typeof(OptionsFactory<>)));
            ServiceRegistrationExpressions.Add(() => ServiceCollection.AddTransient(typeof(OptionsMonitor<>)));

            // Configuration Options
            ServiceRegistrationExpressions.Add(() => ServiceCollection.RegisterConfiguredOptions<ApplicationOptions>(Configuration));
            ServiceRegistrationExpressions.Add(() => ServiceCollection.RegisterConfiguredOptions<InfrastructureOptions>(Configuration));

            // AutoMapper
            ServiceRegistrationExpressions.Add(() => RegisterAutoMapper());
        }

        /// <summary>
        /// Override the default AutoMapper registration.
        /// </summary>
        protected virtual void RegisterAutoMapper()
        {
            ServiceCollection.AddSingleton(serviceProvider =>
            {
                var configurationExpression = new MapperConfigurationExpression();
                AugmentExpressionExecution(() => configurationExpression.ConstructServicesUsing(serviceProvider.GetService));
                AugmentExpressionExecutions(MapperExtensionExpressions, configurationExpression);
                var configuration = new MapperConfiguration(configurationExpression);
                return configuration.CreateMapper();
            });
        }
    }
}
