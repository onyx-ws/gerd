using Microsoft.Extensions.Logging;
using Onyx.Gerd.K8s;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onyx.Gerd
{
    public class Ape : Chimp
    {
        public Ape(ILogger<Gerd> logger) : base(logger) {
            MIN_WAIT = 15;
            MAX_WAIT = 120;
        }
    }
}