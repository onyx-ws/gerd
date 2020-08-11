using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Onyx.Gerd.Tests.K8sMock
{
    class MockK8sApiServer : IDisposable
    {
        private readonly IWebHost _webHost;

        public MockK8sApiServer(string mockResponse = "")
        {
            _webHost = WebHost.CreateDefaultBuilder()
                .Configure(app => app.Run(async httpContext =>
                {
                    await httpContext.Response.WriteAsync(mockResponse);
                }))
                .UseKestrel(options => { options.Listen(IPAddress.Loopback, 0); })
                .Build();

            _webHost.Start();
        }

        public Uri Uri => _webHost.ServerFeatures.Get<IServerAddressesFeature>().Addresses
            .Select(a => new Uri(a)).First();

        public void Dispose()
        {
            _webHost.StopAsync();
            _webHost.WaitForShutdown();
        }
    }
}
