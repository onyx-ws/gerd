using Microsoft.Extensions.Logging;
using Onyx.Gerd.K8s;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onyx.Gerd
{
    public class Chimp : Gerd
    {
        public Chimp(ILogger<Gerd> logger) : base((ILogger<Gerd>)logger)
        {
            MIN_WAIT = 30;
            MAX_WAIT = 300;
        }

        public override async Task<List<Namespace>> GetNamespacesAsync()
        {
            // Honor user defined namespaces; even k8s related
            if (_udTragetNamespaces != null && _udTragetNamespaces.Count > 0)
            {
                return _udTragetNamespaces;
            }

            var k8sNamespaces = await base.GetNamespacesAsync();
            // remove k8s reserved namespaces - kube-system; kube-public; kube-node-lease; etc.
            k8sNamespaces.RemoveAll(ns => ns.Name.StartsWith("kube-", StringComparison.InvariantCultureIgnoreCase));
            return k8sNamespaces;
        }
    }
}