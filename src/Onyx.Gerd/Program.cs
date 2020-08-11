using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Onyx.Gerd
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<GerdWorker>();

                    Enum.TryParse<HavocLevel>(Environment.GetEnvironmentVariable("ONYX_GERD_HAVOC_LEVEL"), out var havocLevel);
                    _ = havocLevel switch
                    {
                        HavocLevel.APE => services.AddSingleton<IGerd, Ape>(),
                        HavocLevel.CHIMP => services.AddSingleton<IGerd, Chimp>(),
                        HavocLevel.GORILLA => services.AddSingleton<IGerd, Gorilla>(),
                        _ => services.AddSingleton<IGerd, Chimp>()
                    };
                    //services.AddSingleton<Gerd>();
                });
    }
}
