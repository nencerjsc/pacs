using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NencerApi.Modules.PacsServer.Model
{
    [Table("DicomInstances")]
    public class DicomInstanceModel
    {
        [Key]
        public string SOPInstanceUID { get; set; }
        public string PatientId { get; set; } = string.Empty;
        public string StudyInstanceUID { get; set; } = string.Empty;
        public string SOPClassUID { get; set; } = string.Empty;

        public string DeviceSerialNumber { get; set; } = string.Empty;
        public string Modality { get; set; } = string.Empty;
        public string Manufacturer { get; set; } = string.Empty;
        public string InstitutionName { get; set; } = string.Empty;

        public string BodyPartExamined { get; set; } = string.Empty;
        public int SeriesNumber { get; set; } = 0;
        public int InstanceNumber { get; set; } = 0;
        public string ImageType { get; set; } = string.Empty;
        public string AcquisitionDate { get; set; } = string.Empty;
        public string AcquisitionTime { get; set; } = string.Empty;
        public string ContentDate { get; set; } = string.Empty;
        public string ContentTime { get; set; } = string.Empty;
        public string ImagePositionPatient { get; set; } = string.Empty;
        public string ImageOrientationPatient { get; set; } = string.Empty;
        public int[]? RowsAndColumns { get; set; }
        public string PhotometricInterpretation { get; set; } = string.Empty;
        public float? WindowCenter { get; set; }
        public float? WindowWidth { get; set; }

        // Các trường mới:
        public string SliceLocation { get; set; } = string.Empty;
        public string PixelSpacing { get; set; } = string.Empty;
        public ushort BitsAllocated { get; set; }
        public ushort BitsStored { get; set; }
        public ushort HighBit { get; set; }
        public string RescaleIntercept { get; set; } = string.Empty;
        public string RescaleSlope { get; set; } = string.Empty;
        public int StorageId { get; set; } = 0;
        public string? FilePath { get; set; }
        public int? NumberOfFrames { get; set; }
        public string? Url { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string SeriesInstanceUID { get; set; } = string.Empty;
        public string TransferSyntaxUID { get; set; } = string.Empty;
       
    }
}
