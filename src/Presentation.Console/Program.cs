using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Hosting;
using Presentation.Console.Commands;
using System;
using System.Threading.Tasks;

#pragma warning disable 1591

namespace Presentation.Console
{
    public static class Program
    {
        public static async Task<int> Main(string[] args)
        {
            IHost host = CreateHostBuilder(args).Build();
            var resultCode = 0;
            try
            {
                resultCode = await host.RunCommandLineApplicationAsync();
            }
            catch (ValidationException e)
            {
                foreach (ValidationFailure validationFailure in e.Errors)
                {
                    System.Console.ForegroundColor = ConsoleColor.Red;
                    System.Console.WriteLine(validationFailure.ErrorMessage);
                    System.Console.ResetColor();
                    resultCode = -1;
                }
            }

            return resultCode;
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
