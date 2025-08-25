using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NencerApi.Modules.PacsServer.Config
{
    public class DicomServerConfig
    {
        public int Port { get; set; } = 11112;
        public string ServerAETitle { get; set; } = "NENCER";
        public string[] AllowedAEs { get; set; } = new[] { "TESTSCU" };
        public string StoragePath { get; set; } = "DICOM_Storage";
    }
}
