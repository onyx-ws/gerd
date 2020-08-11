using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onyx.Gerd.K8s
{
    public class Pod
    {
        public string Name { get; set; }
        public string Status { get; set; }
        public Dictionary<string, string> Labels { get; set; }
    }
}
