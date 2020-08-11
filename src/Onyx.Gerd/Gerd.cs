using Microsoft.Extensions.Logging;
using Onyx.Gerd.K8s;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Onyx.Gerd
{
    public class Gerd : IGerd
    {
        private Random _random;
        protected ILogger<Gerd> _logger;

        private string UD_TARGET_NAMESPACES;
        protected List<Namespace> _udTragetNamespaces;

        protected bool OPT_IN_REQUIRED = true;

        public int MIN_WAIT = 30;
        public int MAX_WAIT = 300;

        public Gerd(ILogger<Gerd> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _random = new Random();

            UD_TARGET_NAMESPACES = Environment.GetEnvironmentVariable("ONYX_GERD_TARGET_NAMESPACES");
            if (!string.IsNullOrWhiteSpace(UD_TARGET_NAMESPACES))
            {
                _udTragetNamespaces = UD_TARGET_NAMESPACES.Split(',')
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .Select(ns => new Namespace() { Name = ns })
                    .ToList();
            }
            if (!string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("ONYX_GERD_OPT_IN_REQURIED")))
            {
                Boolean.TryParse(Environment.GetEnvironmentVariable("ONYX_GERD_OPT_IN_REQURIED"), out OPT_IN_REQUIRED);
            }
        }

        public async Task WreakHavocAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var wakingPeriod = _random.Next(MIN_WAIT, MAX_WAIT);
                _logger.LogInformation($"The Gerd will start wreaking havoc in {wakingPeriod} minutes");

                // Wait for a few minutes before wreaking havoc
                var cancelled = stoppingToken.WaitHandle.WaitOne(1000 * 60 * wakingPeriod);

                if (!cancelled)
                {
                    try
                    {
                        _logger.LogInformation("The Gerd is roaming around.");

                        var namespaces = await GetNamespacesAsync();
                        if (namespaces.Count > 0)
                        {
                            // Shuffle namespaces
                            namespaces = namespaces.Shuffle().ToList();
                    
                            bool havocWreaked = false;
                            foreach (var ns in namespaces)
                            {
                                _logger.LogInformation($"The Gerd is roaming in '{ns.Name}'.");

                                var pods = await GetPodsAsync(ns.Name);
                                if (pods.Count > 0)
                                {
                                    var victim = pods.Where(p => p.Status == "Running").Shuffle().First().Name;
                                    _logger.LogInformation($"The Gerd is pounding '{victim}'.");
                                    await K8sClient.DeleteNamesapcePodsAsync(victim, ns.Name);
                                    havocWreaked = true;
                                }

                                if (havocWreaked) break;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogInformation(ex.ToString());
                    }
                }
            }

            _logger.LogInformation("The Gerd have been sedated.");
        }

        public virtual async Task<List<Namespace>> GetNamespacesAsync()
        {
            return _udTragetNamespaces ?? await K8sClient.ListNamesapcesAsync();
        }

        public virtual async Task<List<Pod>> GetPodsAsync(string namespaceName)
        {
            var pods = await K8sClient.ListNamesapcePodsAsync(namespaceName);
            if (pods.Count > 0)
            {
                if (OPT_IN_REQUIRED)
                {
                    _logger.LogInformation($"opt-in required.");

                    pods = pods.Where(p =>
                            p.Labels.ContainsKey("onyx.gerd.enabled")
                            && Boolean.Parse(p.Labels["onyx.gerd.enabled"])
                        ).ToList();
                }
            }
            return pods;
        }
    }
}