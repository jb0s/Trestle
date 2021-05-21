using System.Collections.Generic;
using System.Net.Sockets;
using Trestle.Configuration.Service;

namespace Trestle.Networking.Services
{
    public interface IClientService
    {
        void RegisterClient(TcpClient tcpClient);

        void UnregisterClient(Client client);
    }
    
    public class ClientService : IClientService
    {
        /// <summary>
        /// List of currently registered clients.
        /// </summary>
        private readonly List<Client> _clients = new();

        private IPacketService _packetService;
        private IMojangService _mojangService;        
        private IConfigService _configService;
        
        public ClientService(IMojangService mojangService, IPacketService packetService, IConfigService configService)
        {
            _mojangService = mojangService;
            _packetService = packetService;
            _configService = configService;
        }
        
        /// <summary>
        /// Registers a client.
        /// </summary>
        /// <param name="tcpClient"></param>
        public void RegisterClient(TcpClient tcpClient)
        {
            var client = new Client(_mojangService, _packetService, this, tcpClient, _configService);
            _clients.Add(client);
        }

        /// <summary>
        /// Unregisters a client.
        /// </summary>
        /// <param name="client">Instance of the Client that will be unregistered.</param>
        public void UnregisterClient(Client client)
        {
            _clients.Remove(client);
        }
    }
}