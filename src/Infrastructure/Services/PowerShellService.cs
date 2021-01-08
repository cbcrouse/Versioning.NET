using Application.Interfaces;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace Infrastructure.Services
{
    public class PowerShellService : IPowerShellService
    {
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
