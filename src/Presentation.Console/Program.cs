using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

#pragma warning disable 1591

namespace Presentation.Console
{
    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            IHost host = CreateHostBuilder(args).Build();
            return await host.RunCommandLineApplicationAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .UseCommandLineApplication<App>(args)
                .ConfigureServices(services =>
                {
                var startup = new Startup();
                startup.ConfigureServices(services);
                });
        }
    }
}
