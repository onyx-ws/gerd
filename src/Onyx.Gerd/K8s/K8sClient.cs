using k8s;
using k8s.KubeConfigModels;
using k8s.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Onyx.Gerd.K8s
{
    public class K8sClient
    {
        private static Kubernetes CLIENT;

        static K8sClient()
        {
            Connect(KubernetesClientConfiguration.BuildDefaultConfig());
        }

        public static void Connect(KubernetesClientConfiguration configuration)
        {
            CLIENT = new Kubernetes(configuration);
        }

        public async static Task<List<Namespace>> ListNamesapcesAsync()
        {
            var k8sNamespaces = await CLIENT.ListNamespaceAsync();
            if (k8sNamespaces == null || k8sNamespaces.Items.Count == 0)
            {
                return new List<Namespace>();
            }

            return k8sNamespaces.Items.Where(ns => ns.Status.Phase == "Active")
                .Select(n => new Namespace()
                {
                    Name = n.Name(),
                    Status = n.Status.Phase
                })
                .ToList();
        }

        public async static Task<List<Pod>> ListNamesapcePodsAsync(string namespaceName)
        {
            var pods = await CLIENT.ListNamespacedPodAsync(namespaceName);
            //return pods.Items.Where(p => p.GetLabel("gerd/enabled") == "true").Select(p => new Pod() { Name = p.Name(), Status = p.Status.Phase }).ToList();
            return pods.Items
                .Select(p => new Pod()
                {
                    Name = p.Name(),
                    Status = p.Status.Phase,
                    Labels = (Dictionary<string, string>)p.Labels()
                })
                .ToList();
        }

        public async static Task DeleteNamesapcePodsAsync(string podName, string namespaceName)
        {
            var client = new Kubernetes(KubernetesClientConfiguration.BuildDefaultConfig());
            await client.DeleteNamespacedPodAsync(podName, namespaceName, new V1DeleteOptions() { GracePeriodSeconds = 0 });
        }
    }
}