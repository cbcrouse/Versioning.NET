using Microsoft.Extensions.Hosting;
using System.Reflection;
using System.Threading.Tasks;

#pragma warning disable 1591

namespace Presentation.Console
{
    public class Program
	{
		public static async Task Main(string[] args)
		{
			var builder = CreateHostBuilder(args).Build();

			DisplayIntro();
			// ParseArgs();

			await builder.RunAsync();
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.ConfigureServices(services =>
				{
					var startup = new Startup();
					startup.ConfigureServices(services);
				});

		private static void DisplayIntro()
		{
			string version = Assembly.GetEntryAssembly()!.GetCustomAttribute<AssemblyInformationalVersionAttribute>()!.InformationalVersion;
			System.Console.WriteLine($"Current version: {version}");
		}
	}
}
