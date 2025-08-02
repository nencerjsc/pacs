namespace NencerApi.Modules.PacsServer.Model
{
    public class DicomInstanceModel
    {
        public string PatientId { get; set; } = string.Empty;
        public string SOPInstanceUID { get; set; } = string.Empty;
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
        public DicomSerieModel Serie { get; set; } = null!;

        public string TransferSyntaxUID { get; set; } = string.Empty;
        public ICollection<DicomTagItemModel> Tags { get; set; } = new List<DicomTagItemModel>();

        public Dictionary<string, object> ToDicomJson()
        {
            var json = new Dictionary<string, object>();

            if (!string.IsNullOrEmpty(SOPInstanceUID))
                json["00080018"] = new { vr = "UI", Value = new[] { SOPInstanceUID } };

            if (!string.IsNullOrEmpty(StudyInstanceUID))
                json["0020000D"] = new { vr = "UI", Value = new[] { StudyInstanceUID } };

            if (!string.IsNullOrEmpty(SeriesInstanceUID))
                json["0020000E"] = new { vr = "UI", Value = new[] { SeriesInstanceUID } };

            if (!string.IsNullOrEmpty(SOPClassUID))
                json["00080016"] = new { vr = "UI", Value = new[] { SOPClassUID } };

            if (!string.IsNullOrEmpty(Modality))
                json["00080060"] = new { vr = "CS", Value = new[] { Modality } };

            //if (NumberOfFrames != 0)
            //    json["00280008"] = new { vr = "IS", Value = new[] { NumberOfFrames.ToString() } };

            if (SeriesNumber != 0)
                json["00200011"] = new { vr = "IS", Value = new[] { SeriesNumber } };

            if (InstanceNumber != 0)
                json["00200013"] = new { vr = "IS", Value = new[] { InstanceNumber } };

            if (!string.IsNullOrEmpty(ImageType))
                json["00080008"] = new { vr = "CS", Value = ImageType.Split('\\') };

            if (!string.IsNullOrEmpty(AcquisitionDate))
                json["00080022"] = new { vr = "DA", Value = new[] { AcquisitionDate } };

            if (!string.IsNullOrEmpty(AcquisitionTime))
                json["00080032"] = new { vr = "TM", Value = new[] { AcquisitionTime } };

            if (!string.IsNullOrEmpty(ContentDate))
                json["00080023"] = new { vr = "DA", Value = new[] { ContentDate } };

            if (!string.IsNullOrEmpty(ContentTime))
                json["00080033"] = new { vr = "TM", Value = new[] { ContentTime } };

            if (!string.IsNullOrEmpty(ImagePositionPatient))
                json["00200032"] = new { vr = "DS", Value = ImagePositionPatient };

            if (!string.IsNullOrEmpty(ImageOrientationPatient))
                json["00200037"] = new { vr = "DS", Value = ImageOrientationPatient };

            if (RowsAndColumns != null && RowsAndColumns.Length == 2)
            {
                json["00280010"] = new { vr = "US", Value = new[] { RowsAndColumns[0] } }; // Rows
                json["00280011"] = new { vr = "US", Value = new[] { RowsAndColumns[1] } }; // Columns
            }

            if (!string.IsNullOrEmpty(PhotometricInterpretation))
                json["00280004"] = new { vr = "CS", Value = new[] { PhotometricInterpretation } };

            if (WindowCenter != null)
                json["00281050"] = new { vr = "DS", Value = new[] { WindowCenter } };

            if (WindowWidth != null)
                json["00281051"] = new { vr = "DS", Value = new[] { WindowWidth } };

            if (!string.IsNullOrEmpty(SliceLocation))
                json["00201041"] = new { vr = "DS", Value = new[] { SliceLocation } };

            if (PixelSpacing != null && PixelSpacing.Length == 2)
                json["00280030"] = new { vr = "DS", Value = PixelSpacing };

            if (BitsAllocated > 0)
                json["00280100"] = new { vr = "US", Value = new[] { BitsAllocated } };

            if (BitsStored > 0)
                json["00280101"] = new { vr = "US", Value = new[] { BitsStored } };

            if (HighBit > 0)
                json["00280102"] = new { vr = "US", Value = new[] { HighBit } };

            if (!string.IsNullOrEmpty(RescaleIntercept))
                json["00281052"] = new { vr = "DS", Value = new[] { RescaleIntercept } };

            if (!string.IsNullOrEmpty(RescaleSlope))
                json["00281053"] = new { vr = "DS", Value = new[] { RescaleSlope } };

            if (!string.IsNullOrEmpty(TransferSyntaxUID))
                json["00020010"] = new { vr = "DS", Value = new[] { TransferSyntaxUID } };

            return json;
        }
    }
}
