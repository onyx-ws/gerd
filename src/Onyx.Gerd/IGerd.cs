using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Onyx.Gerd
{
    public interface IGerd
    {
        Task WreakHavocAsync(CancellationToken stoppingToken);
    }
}
