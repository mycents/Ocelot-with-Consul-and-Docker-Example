using Infra.Shared.StaticLogger;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;

namespace Infra.Shared.ServiceDiscovery
{
    public static class ServiceConfigExtensions
    {
        public static ServiceConfig GetServiceConfig(this IConfiguration configuration)
        {
            var log = AppLogger.CreateLogger<ServiceConfig>();
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var serviceDiscoveryAddress = configuration.GetValue<Uri>("ServiceConfig:serviceDiscoveryAddress");
            var serviceAddress = configuration.GetValue<Uri>("ServiceConfig:serviceAddress");
            var serviceName = configuration.GetValue<string>("ServiceConfig:serviceName");

            log.LogDebug("==================================================================");
            log.LogDebug("ServiceConfigExtensions");
            log.LogDebug($" > GetServiceConfig({serviceName}):");
            log.LogDebug($" ==> serviceDiscoveryAddress = {serviceDiscoveryAddress}");
            log.LogDebug($" ==> serviceAddress          = {serviceAddress}");
            log.LogDebug("==================================================================");

            var serviceConfig = new ServiceConfig
            {
                ServiceDiscoveryAddress = serviceDiscoveryAddress,
                ServiceAddress = serviceAddress,
                ServiceName = serviceName,
            };

            return serviceConfig;
        }
    }
}