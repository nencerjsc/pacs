using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace NencerApi.Modules.PacsServer.Service
{
    public class DicomListenerHostedService : IHostedService
    {
        private readonly ILogger<DicomListenerHostedService> _logger;
        private readonly DicomListenerManager _listener;

        public DicomListenerHostedService(ILogger<DicomListenerHostedService> logger, DicomListenerManager listener)
        {
            _logger = logger;
            _listener = listener;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("🚀 HostedService bắt đầu Listener");
            _listener.Start();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("🛑 Dừng HostedService Listener");
            _listener.Stop();
            return Task.CompletedTask;
        }
    }
}
