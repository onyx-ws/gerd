using k8s;
using Onyx.Gerd.K8s;
using Onyx.Gerd.Tests.K8sMock;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            // Arrange
            var k8sMockResponse = @"{""kind"":""NamespaceList"",""apiVersion"":""v1"",""metadata"":{""selfLink"":""/api/v1/namespaces"",""resourceVersion"":""1762810""},""items"":[{""metadata"":{""name"":""gerd"",""selfLink"":""/api/v1/namespaces/gerd"",""uid"":""ac1abb94-9c58-11e7-aaf5-00155d744505"",""resourceVersion"":""1737928"",""creationTimestamp"":""2017-09-18T10:03:51Z"",""labels"":{""name"":""gerd""}},""status"":{""phase"":""Active""}}]}";

            // Act
            using (var server = new MockK8sApiServer(mockResponse: k8sMockResponse))
            {
                var config = new KubernetesClientConfiguration { Host = server.Uri.ToString() };
                K8sClient.ConnectAsync(config);
                for (int i = 0; i < 100000; i++)
                {
                    Thread.Sleep(100);
                    var namespaces = new List<Namespace>();
                    namespaces = K8sClient.ListNamesapcesAsync().Result;
                }

            }
        }
    }
}
