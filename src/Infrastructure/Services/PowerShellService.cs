using Application.Interfaces;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace Infrastructure.Services
{
    /// <summary>
    /// Provides the implementation details for interacting with PowerShell.
    /// </summary>
    public class PowerShellService : IPowerShellService
    {
        private readonly ILogger<PowerShellService> _logger;

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="logger">A generic interface for logging.</param>
        public PowerShellService(ILogger<PowerShellService> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Run a PowerShell script.
        /// </summary>
        /// <param name="location">The directory to run the script from.</param>
        /// <param name="script">The script to execute.</param>
        public Collection<PSObject> RunScript(string location, string script)
        {
            using Runspace runspace = RunspaceFactory.CreateRunspace();
            runspace.Open();
            runspace.SessionStateProxy.Path.SetLocation(location);

            using Pipeline pipeline = runspace.CreatePipeline();

            pipeline.Commands.AddScript(script);
            _logger.LogInformation($"Executing script: '{script}'");
            Collection<PSObject> result = pipeline.Invoke();
            runspace.Close();
            foreach (PSObject psObject in result)
            {
                _logger.LogInformation(psObject.ToString());
            }
            return result;
        }
    }
}
