using Application.Configuration;
using Application.Interfaces;
using FluentValidation;
using Infrastructure.MediatR;
using Infrastructure.Services;
using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Extensions.Logging;
using ServiceComposition.NET;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Infrastructure.Startup;

/// <summary>
/// Facilitates dependency registrations for the application.
/// </summary>
public class CoreServiceRegistrationPipeline : ServiceRegistrationPipeline
{
    /// <summary>
    /// Default Constructor
    /// </summary>
    public CoreServiceRegistrationPipeline()
    {
        // Configuration Options
        AddRegistration(services => services.AddOptions());
        AddRegistration(services => services.AddTransient(typeof(OptionsFactory<>)));
        AddRegistration(services => services.AddTransient(typeof(OptionsMonitor<>)));

        // Add MediatR and FluentValidation
        AddRegistration(services => services.AddTransient(typeof(IRequestPreProcessor<>), typeof(RequestLogger<>)));
        AddRegistration(services => services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestPreProcessorBehavior<,>)));
        AddRegistration(services => services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehavior<,>)));
        AddRegistration(services => services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestMetricsBehavior<,>)));
        AddRegistration(services => services.AddMediatR(mediatRServiceConfiguration => mediatRServiceConfiguration.RegisterServicesFromAssembly(typeof(ApplicationOptions).Assembly)));
        AddRegistration(services => services.AddValidatorsFromAssemblyContaining(typeof(ApplicationOptions), ServiceLifetime.Transient));

        AddRegistration(services => services.AddSingleton<ISystemClock, SystemClock>());

        //Services
        AddRegistration(services => services.AddSingleton<IGitService, LibGit2Service>());
        AddRegistration(services => services.AddSingleton<IGitVersioningService, GitVersioningService>());
        AddRegistration(services => services.AddSingleton<IAssemblyVersioningService, AssemblyVersioningService>());
    }

    /// <inheritdoc />
    protected override ILogger StartupLogger =>
        new SerilogLoggerFactory(
            new LoggerConfiguration()
                .Enrich.FromLogContext()
#if DEBUG
                .MinimumLevel.Verbose()
#else
                .MinimumLevel.Error()
#endif
                .WriteTo.Console(outputTemplate: "[{Timestamp:yyyy:MM:dd hh:mm:ss.fff tt}] [{Level}] [{SourceContext}] {Message}{NewLine}{Exception}")
                .CreateLogger()
        ).CreateLogger(nameof(CoreServiceRegistrationPipeline));
}