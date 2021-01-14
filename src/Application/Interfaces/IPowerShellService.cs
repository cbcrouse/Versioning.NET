using System.Collections.ObjectModel;
using System.Management.Automation;

namespace Application.Interfaces
{
    /// <summary>
    /// An abstraction to facilitate testing without using the PowerShell integration.
    /// </summary>
    public interface IPowerShellService
    {
        /// <summary>
        /// Run a PowerShell script.
        /// </summary>
        /// <param name="location">The directory to run the script from.</param>
        /// <param name="script">The script to execute.</param>
        Collection<PSObject> RunScript(string location, string script);
    }
}
