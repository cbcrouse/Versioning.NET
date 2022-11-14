using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integration.Tests.Setup
{
    partial class EnvironmentVariable
    {
        public static EnvironmentVariable local = new EnvironmentVariable();

        public string GitHubActor = null;
        public string GitHubAccessToken = null;
        public string GitHubTestRepoAddress = null;
    }
}
