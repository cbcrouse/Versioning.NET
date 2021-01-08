using System.Collections.ObjectModel;
using System.Management.Automation;

namespace Application.Interfaces
{
    public interface IPowerShellService
    {
        Collection<PSObject> RunScript(string location, string script);
    }
}
