using Consul;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Infra.Shared.ServiceDiscovery
{
    public class ServiceDiscoveryHostedService : IHostedService
    {
        private readonly IConsulClient _client;
        private readonly ServiceConfig _config;
        private readonly ILogger<ServiceDiscoveryHostedService> _logger;
        private string _registrationId { get; set; }




        public ServiceDiscoveryHostedService(
            IConsulClient client,
            ServiceConfig config,
            ILogger<ServiceDiscoveryHostedService> logger)
        {
            _client = client;
            _config = config;
            _logger = logger;
        }

        private string getLocalIpv4(string hostName = "")
        {
            // Obtem o nome do host da máquina local
            if (string.IsNullOrWhiteSpace(hostName))
            {
                hostName = Dns.GetHostName();
            }

            // Obtem a entrada do host para o nome do host
            IPHostEntry hostEntry = Dns.GetHostEntry(hostName);

            // Filtra a lista de endereços IP para encontrar o primeiro IPv4
            IPAddress ipv4Address = null;
            foreach (IPAddress address in hostEntry.AddressList)
            {
                if (address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    ipv4Address = address;
                    break;
                }
            }

            // Verifica se um endereço IPv4 foi encontrado e o exibe
            if (ipv4Address != null)
            {
                Console.WriteLine("Endereço IPv4 encontrado: " + ipv4Address.ToString());
            }
            else
            {
                Console.WriteLine("Nenhum endereço IPv4 encontrado.");
            }

            return ipv4Address?.ToString() ?? "localhost";
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogWarning("==================================================================");
            _logger.LogWarning(" > Registering with Consul");

            var containerId = Environment.MachineName; // O hostname dentro de um container Docker geralmente corresponde ao ID do container.

            //var dockerService = new DockerService();
            //var serviceName = await dockerService.GetContainerNameAsync(containerId);

            _registrationId = $"{_config.ServiceName}-{containerId}"; ;// _config.ServiceName + "-" + Guid.NewGuid();
            var serviceName = _config.ServiceName;
            var servicePort = _config.ServiceAddress.Port;
            //var serviceIp = Dns.GetHostEntry(Dns.GetHostName()).AddressList[0];
            //var serviceIp = Dns.GetHostEntry(_config.ServiceAddress.Host).AddressList[0];
            //var serviceIp = getLocalIpv4(_config.ServiceAddress.Host);
            var serviceIp = getLocalIpv4();

            _logger.LogWarning($" => serviceId..: {_registrationId}");
            _logger.LogWarning($" => serviceName: {serviceName}");
            _logger.LogWarning($" => servicePort: {servicePort}");
            _logger.LogWarning($" => serviceIp..: {serviceIp}");

            var registration = new AgentServiceRegistration
            {
                ID = _registrationId,
                Name = serviceName,
                Address = serviceIp.ToString(),
                Port = servicePort,
                Check = new AgentCheckRegistration()
                {
                    HTTP = $"http://{serviceIp}:{servicePort}/_healthcheck/status",
                    Interval = TimeSpan.FromSeconds(10)
                }
            };

            await _client.Agent.ServiceDeregister(registration.ID, cancellationToken);
            await _client.Agent.ServiceRegister(registration, cancellationToken);

            _logger.LogWarning("==================================================================");
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogWarning("==================================================================");
            _logger.LogWarning(" > Unregistering from Consul");
            _logger.LogWarning("==================================================================");

            await _client.Agent.ServiceDeregister(_registrationId, cancellationToken);
        }
    }
}