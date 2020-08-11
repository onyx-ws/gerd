using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Onyx.Gerd
{
    public class GerdWorker : BackgroundService
    {
        private readonly ILogger<GerdWorker> _logger;
        private readonly IGerd _gerd;

        public GerdWorker(ILogger<GerdWorker> logger, IGerd gerd)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _gerd = gerd ?? throw new ArgumentNullException(nameof(gerd));

            _logger.LogInformation("The Gerd is waking up.");
            
            var envUdNamespaces = Environment.GetEnvironmentVariable("ONYX_GERD_TARGET_NAMESPACES");
            if (string.IsNullOrWhiteSpace(envUdNamespaces))
            {
                _logger.LogInformation("No user defined namespaces");
            }
            else
            {
                _logger.LogInformation($"User defined namespaces {envUdNamespaces}");
            }

            _logger.LogInformation($"Will summon a(n) {gerd.GetType().Name}");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Delay(100, stoppingToken);
            
            await _gerd.WreakHavocAsync(stoppingToken);
        }
    }
}