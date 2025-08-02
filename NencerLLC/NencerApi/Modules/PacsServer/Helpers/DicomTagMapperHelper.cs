using NencerApi.Modules.PacsServer.Model;

namespace NencerApi.Modules.PacsServer.Helpers
{
    public class DicomTagMapperHelper
    {
        public static readonly Dictionary<string, (string vr, Func<DicomStudyModel, object?> getter)> StudyTagMap = new()
        {
            { "00100020", ("LO", s => s.PatientID) },
            { "00100010", ("PN", s => s.PatientName) },
            { "00100040", ("CS", s => s.PatientSex) },
            { "00100030", ("DA", s => s.PatientBirthDate) },
            { "00080050", ("SH", s => s.AccessionNumber) },
            { "00080020", ("DA", s => s.StudyDate) },
            { "00080030", ("TM", s => s.StudyTime) },
            { "0020000D", ("UI", s => s.StudyInstanceUID) },
            { "00200010", ("SH", s => s.StudyID) },
            { "00081030", ("LO", s => s.StudyDescription) },
            { "00080061", ("CS", s => s.ModalitiesInStudy) },
            { "00201206", ("IS", s => s.NumberOfStudyRelatedSeries) },
            { "00201208", ("IS", s => s.NumberOfStudyRelatedInstances) }
        };

        public static readonly Dictionary<string, (string vr, Func<DicomSerieModel, object?> getter)> SeriesTagMap = new()
        {
            { "0020000D", ("UI", s => s.StudyInstanceUID) },
            { "00200011", ("IS", s => s.SeriesNumber) },
            { "0020000E", ("UI", s => s.SeriesInstanceUID) },
            { "00080060", ("CS", s => s.Modality) },
            { "00080021", ("DA", s => s.StudyDate) },
            { "00080031", ("TM", s => s.StudyTime) },
            { "00080070", ("LO", s => s.Manufacturer) },
            { "00080080", ("LO", s => s.InstitutionName) },
            { "00181030", ("LO", s => s.SeriesDescription) },
            { "00081090", ("LO", s => s.SopClass) }
        };

        public static readonly Dictionary<string, (string vr, Func<DicomInstanceModel, object?> getter)> InstanceTagMap = new()
        {
            { "00080018", ("UI", i => i.SOPInstanceUID) },
            { "0020000D", ("UI", i => i.StudyInstanceUID) },
            { "0020000E", ("UI", i => i.SeriesInstanceUID) },
            { "00080016", ("UI", i => i.SOPClassUID) },
            { "00080060", ("CS", i => i.Modality) },
            { "00200011", ("IS", i => i.SeriesNumber) },
            { "00200013", ("IS", i => i.InstanceNumber) },
            { "00080008", ("CS", i => i.ImageType?.Split('\\')) },
            { "00080022", ("DA", i => i.AcquisitionDate) },
            { "00080032", ("TM", i => i.AcquisitionTime) },
            { "00080023", ("DA", i => i.ContentDate) },
            { "00080033", ("TM", i => i.ContentTime) },
            { "00200032", ("DS", i => i.ImagePositionPatient) },
            { "00200037", ("DS", i => i.ImageOrientationPatient) },
            { "00280010", ("US", i => i.RowsAndColumns?[0]) },
            { "00280011", ("US", i => i.RowsAndColumns?[1]) },
            { "00280004", ("CS", i => i.PhotometricInterpretation) },
            { "00281050", ("DS", i => i.WindowCenter) },
            { "00281051", ("DS", i => i.WindowWidth) },
            { "00201041", ("DS", i => i.SliceLocation) },
            { "00280030", ("DS", i => i.PixelSpacing) },
            { "00280100", ("US", i => i.BitsAllocated) },
            { "00280101", ("US", i => i.BitsStored) },
            { "00280102", ("US", i => i.HighBit) },
            { "00281052", ("DS", i => i.RescaleIntercept) },
            { "00281053", ("DS", i => i.RescaleSlope) },
            { "00020010", ("UI", i => i.TransferSyntaxUID) },
            { "7FE00010", ("OB|OW", i => $"instances/{i.SOPInstanceUID}/frames") }
        };

        public static readonly Dictionary<string, (string vr, Func<DicomWorkListModel, object?> getter)> WorkListTagMap =
        new()
        {
            // Bệnh nhân
            { "00100010", ("PN", w => $"{w.Surname}^{w.Forename}") },            // PatientName (DICOM format: Family^Given)
            { "00100020", ("LO", w => w.PatientID) },                            // PatientID
            { "00100040", ("CS", w => w.PatientSex) },                           // PatientSex
            { "00100030", ("DA", w => w.PatientBirthDate) },                     // PatientBirthDate

            // Thủ tục yêu cầu
            { "00080050", ("SH", w => w.AccessionNumber) },                      // AccessionNumber
            { "00080060", ("CS", w => w.Modality) },                             // Modality
            { "0020000D", ("UI", w => w.StudyInstanceUID) },                     // StudyInstanceUID
            { "00401001", ("SH", w => w.RequestedProcedureID) },                 // RequestedProcedureID
            { "00321060", ("LO", w => w.RequestedProcedureDescription) },        // ReasonForStudy

            // Thời gian
            { "00080020", ("DA", w => w.StudyDate) },                            // StudyDate

            // Thông tin phòng khám, bệnh viện
            { "00081010", ("LO", w => w.HospitalName) },                         // InstitutionName
            { "00081030", ("LO", w => w.ExamDescription) },                      // StudyDescription
            { "00081040", ("LO", w => w.ExamRoom) },                             // ExamRoom (StationName)

            // Order ID nội bộ
            { "00400101", ("SH", w => w.OrderRequestId) }                        // Custom: Order ID
        };
    }
}
