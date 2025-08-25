using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using FellowOakDicom.Network;
using Microsoft.Extensions.Configuration;

using Serilog;

namespace NencerApi.Modules.PacsServer.Config
{
    public static class AppConfig
    {
        public static DicomServerConfig DicomServer { get; }
        public static DatabaseConfig Database { get; set; }

        static AppConfig()
        {
            try
            {
                var config = new ConfigurationBuilder()
                             .SetBasePath(Directory.GetCurrentDirectory())
                             .AddJsonFile("appsettings.json")
                             .Build();

                // Load cấu hình DicomServer
                var dicomServerOptions = config.GetSection("DicomServer").Get<DicomServerConfig>();
                if (dicomServerOptions == null)
                    throw new Exception("Không đọc được cấu hình từ section DicomServer.");
                DicomServer = dicomServerOptions;

                //Log.Information("🟢 Đã load cấu hình DICOM từ file DicomServerConfig.json.");

                // Load cấu hình Database
                var databaseOptions = config.GetSection("ConnectionStrings").Get<DatabaseConfig>();
                if (databaseOptions == null)
                    throw new Exception("Không đọc được cấu hình từ section Database.");
                Database = databaseOptions;

                //Log.Information("🟢 Đã load cấu hình Database từ file DicomServerConfig.json.");

            }
            catch (Exception ex)
            {
                Log.Error(ex, "⚠️ Không đọc được DicomServerConfig.json hoặc dữ liệu không hợp lệ. Sử dụng cấu hình mặc định.");
                DicomServer = new DicomServerConfig();
                Database = new DatabaseConfig();
            }
        }
    }
}
