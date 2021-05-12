using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Trestle.Networking.Services
{
    public class ListenerService : IHostedService
    {
        /// <summary>
        /// Listens on the provided socket.
        /// </summary>
        private readonly TcpListener _tcpListener = new TcpListener(IPAddress.Any, 25565);

        /// <summary>
        /// Thread that the <see cref="TcpListener"/> lives on.
        /// </summary>
        private bool _isListening;
        
        /// <summary>
        /// Thread that the <see cref="TcpListener"/> lives on.
        /// </summary>
        private readonly Thread _thread;
        
        private readonly IClientService _clientService;

        public ListenerService(IClientService clientService)
        {
            _clientService = clientService;
            
            _thread = new Thread(async () =>
            {
                _tcpListener.Start();

                while (_isListening)
                {
                    var client = await _tcpListener.AcceptTcpClientAsync();
                    new Task(() => _clientService.RegisterClient(client)).Start();
                }
            });
        }
        
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _isListening = true;
            _thread.Start();
            
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _isListening = false;
            _thread.Interrupt();
            
            return Task.CompletedTask;
        }
    }
}