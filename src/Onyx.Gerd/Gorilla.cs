using Microsoft.Extensions.Logging;
using Onyx.Gerd.K8s;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onyx.Gerd
{
    public class Gorilla : Ape
    {
        public Gorilla(ILogger<Gerd> logger) : base(logger)
        {
            MIN_WAIT = 5;
            MAX_WAIT = 60;
            OPT_IN_REQUIRED = false; // The Gorilla doesn't care about opt-in; like it or not, you are getting pounded
        }

        public override async Task<List<Namespace>> GetNamespacesAsync()
        {
            return _udTragetNamespaces ?? await K8sClient.ListNamesapcesAsync();
        }
    }
}