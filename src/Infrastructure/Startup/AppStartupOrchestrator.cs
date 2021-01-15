using Application.Configuration;
using Application.Interfaces;
using Application.Interfaces.ValueResolvers;
using Application.Mapping;
using AutoMapper;
using AutoMapper.Configuration;
using FluentValidation;
using Infrastructure.Configuration;
using Infrastructure.MediatR;
using Infrastructure.Services;
using Infrastructure.ValueResolvers;
using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Internal;
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
			// Configuration Options
			ServiceRegistrationExpressions.Add(() => ServiceCollection.AddOptions());
			ServiceRegistrationExpressions.Add(() => ServiceCollection.AddTransient(typeof(OptionsFactory<>)));
			ServiceRegistrationExpressions.Add(() => ServiceCollection.AddTransient(typeof(OptionsMonitor<>)));
			ServiceRegistrationExpressions.Add(() => ServiceCollection.RegisterConfiguredOptions<ApplicationOptions>(Configuration));
			ServiceRegistrationExpressions.Add(() => ServiceCollection.RegisterConfiguredOptions<InfrastructureOptions>(Configuration));

			// Add MediatR and FluentValidation
			ServiceRegistrationExpressions.Add(() => ServiceCollection.AddTransient(typeof(IRequestPreProcessor<>), typeof(RequestLogger<>)));
			ServiceRegistrationExpressions.Add(() => ServiceCollection.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestPreProcessorBehavior<,>)));
			ServiceRegistrationExpressions.Add(() => ServiceCollection.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehavior<,>)));
			ServiceRegistrationExpressions.Add(() => ServiceCollection.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestMetricsBehavior<,>)));
			ServiceRegistrationExpressions.Add(() => ServiceCollection.AddMediatR(typeof(ApplicationOptions).Assembly));
			ServiceRegistrationExpressions.Add(() => ServiceCollection.AddValidatorsFromAssemblyContaining(typeof(ApplicationOptions), ServiceLifetime.Transient));

			// AutoMapper
			ServiceRegistrationExpressions.Add(() => RegisterAutoMapper());

			ServiceRegistrationExpressions.Add(() => ServiceCollection.AddSingleton<ISystemClock, SystemClock>());

			//Services
			ServiceRegistrationExpressions.Add(() => ServiceCollection.AddSingleton<IGitService, GitService>());
			ServiceRegistrationExpressions.Add(() => ServiceCollection.AddSingleton<IGitVersioningService, GitVersioningService>());
			ServiceRegistrationExpressions.Add(() => ServiceCollection.AddSingleton<IAssemblyVersioningService, AssemblyVersioningService>());
			ServiceRegistrationExpressions.Add(() => ServiceCollection.AddSingleton<IPowerShellService, PowerShellService>());
		}

		private void RegisterAutoMapper()
		{
			ServiceCollection.AddSingleton(serviceProvider =>
			{
				var configurationExpression = new MapperConfigurationExpression();
				AugmentExpressionExecution(() => configurationExpression.ConstructServicesUsing(serviceProvider.GetService));
				AugmentExpressionExecutions(MapperExtensionExpressions, configurationExpression);
				var configuration = new MapperConfiguration(configurationExpression);
				return configuration.CreateMapper();
			});

			// Profiles
			MapperExtensionExpressions.Add(mapperConfig => mapperConfig.AddProfile(typeof(ApplicationProfile)));

			// AutoMapper Resolvers
			ServiceCollection.AddSingleton(typeof(INowValueResolver<,>), typeof(NowValueResolver<,>));
		}
	}
}
