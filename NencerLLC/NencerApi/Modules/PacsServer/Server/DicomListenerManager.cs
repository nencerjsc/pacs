using FellowOakDicom.Network;
using Microsoft.Extensions.Logging;
using NencerApi.Modules.PacsServer.Config;
using System;

namespace NencerApi.Modules.PacsServer.Server
{
    public class DicomListenerManager
    {
        private readonly ILogger<DicomListenerManager> _logger;
        private IDicomServer _dicomServer;
        private int _port = 11112;
        private string _aeTitle = "NENCER";

        public bool IsRunning => _dicomServer != null;

        public DicomListenerManager(ILogger<DicomListenerManager> logger)
        {
            _logger = logger;
        }

        public void Start()
        {
            if (IsRunning)
            {
                _logger.LogWarning("⚠️ Listener đã chạy.");
                return;
            }
            _port = AppConfig.DicomServer.Port;
            _aeTitle = AppConfig.DicomServer.ServerAETitle;
            try
            {
                _dicomServer = DicomServerFactory.Create<DicomCStoreSCP>(_port);
                _logger.LogInformation("✅ Listener đã chạy trên cổng {Port} - AE: {AeTitle}", _port, _aeTitle);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Lỗi khi khởi động Listener.");
            }
        }

        public void Stop()
        {
            if (!IsRunning)
            {
                _logger.LogWarning("⚠️ Listener chưa chạy.");
                return;
            }

            try
            {
                _dicomServer?.Dispose();
                _dicomServer = null;
                _logger.LogInformation("🛑 Listener đã dừng.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Lỗi khi dừng Listener.");
            }
        }
    }
}
