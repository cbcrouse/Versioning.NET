using Infrastructure.Startup;
using ServiceComposition.NET;

namespace Versioning.NET;

/// <summary>
/// Provides a startup orchestrator for the console application.
/// </summary>
public class AppServiceComposition : ServiceCompositionRoot<CoreServiceRegistrationPipeline>;