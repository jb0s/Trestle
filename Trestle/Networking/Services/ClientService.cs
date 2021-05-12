using System.Collections.Generic;
using System.Net.Sockets;

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
        
        public ClientService(IPacketService packetService)
        {
            _packetService = packetService;
        }
        
        /// <summary>
        /// Registers a client.
        /// </summary>
        /// <param name="tcpClient"></param>
        public void RegisterClient(TcpClient tcpClient)
        {
            var client = new Client(this, _packetService, tcpClient);
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