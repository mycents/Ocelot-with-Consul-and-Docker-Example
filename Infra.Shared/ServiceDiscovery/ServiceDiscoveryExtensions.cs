using Consul;
using Infra.Shared.StaticLogger;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;

namespace Infra.Shared.ServiceDiscovery
{
    public static class ServiceDiscoveryExtensions
    {
        public static void RegisterConsulServices(this IServiceCollection services, ServiceConfig serviceConfig)
        {
            var log = AppLogger.CreateLogger("RegisterConsulServices()");

            log.LogWarning($" > RegisterConsulServices() ==================================================================");
            log.LogWarning($" >> param:serviceConfig");
            log.LogWarning($" ==> ServiceDiscoveryAddress: {serviceConfig.ServiceDiscoveryAddress}");
            log.LogWarning($" ==> ServiceName:{serviceConfig.ServiceName}");
            log.LogWarning($" ==> ServiceAddress:{serviceConfig.ServiceAddress}");
            log.LogWarning("==================================================================");

            if (serviceConfig == null)
            {
                throw new ArgumentNullException(nameof(serviceConfig));
            }

            var consulClient = CreateConsulClient(serviceConfig);

            services.AddSingleton(serviceConfig);
            services.AddSingleton<IHostedService, ServiceDiscoveryHostedService>();
            services.AddSingleton<IConsulClient, ConsulClient>(p => consulClient);
        }

        private static ConsulClient CreateConsulClient(ServiceConfig serviceConfig)
        {
            var log = AppLogger.CreateLogger("CreateConsulClient()");
            log.LogWarning("==================================================================");

            log.LogWarning($" > CreateConsulClient(serviceConfig)");
            log.LogWarning($"   | CreateConsulClient");
            log.LogWarning($"   | config.Address = serviceConfig.ServiceDiscoveryAddress: {serviceConfig.ServiceDiscoveryAddress}");
            log.LogWarning("==================================================================");


            return new ConsulClient(config =>
            {
                config.Address = serviceConfig.ServiceDiscoveryAddress;
            });
        }
    }
}