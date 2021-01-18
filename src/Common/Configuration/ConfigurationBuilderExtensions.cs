using Microsoft.Extensions.Configuration;

namespace Common.Configuration
{
    /// <summary>
    /// Provides extended functionality for <see cref="IConfigurationBuilder"/>.
    /// </summary>
    public static class ConfigurationBuilderExtensions
	{
		/// <summary>
		/// Add several json file providers environment variables to the configuration builder in a specific order.
		/// </summary>
		/// <param name="builder">Represents a type used to build application configuration.</param>
		public static IConfigurationBuilder AddPrioritizedSettings(this IConfigurationBuilder builder)
		{
			// Order matters here
			builder.AddJsonFile("appsettings.core.json", optional: false, reloadOnChange: true);
			builder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
			builder.AddEnvironmentVariables();
			builder.AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true);

			return builder;
		}
	}
}
