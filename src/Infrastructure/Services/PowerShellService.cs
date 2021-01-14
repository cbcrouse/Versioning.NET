using Application.Interfaces;
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
            Collection<PSObject> result = pipeline.Invoke();
            runspace.Close();
            return result;
        }
    }
}
