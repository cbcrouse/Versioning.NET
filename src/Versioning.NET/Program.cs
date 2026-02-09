
using FluentValidation.Results;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;
using FluentValidation;
using Versioning.NET.Commands;

#pragma warning disable 1591

namespace Versioning.NET;

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
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(validationFailure.ErrorMessage);
                Console.ResetColor();
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