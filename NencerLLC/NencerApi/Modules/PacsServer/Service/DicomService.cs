using FellowOakDicom;
using FellowOakDicom.IO;
using FellowOakDicom.IO.Reader;
using FellowOakDicom.Log;
using NencerApi.Modules.PacsServer.Model;


namespace NencerApi.Modules.PacsServer.Service
{
    public class DicomService
    {
        private readonly string _dicomStorePath = @"C:\Pacs\Storage";

        /// <summary>
        /// Lấy tất cả DICOM Dataset từ thư mục lưu trữ
        /// </summary>
        public IEnumerable<DicomDataset> GetAllStudies()
        {
            var files = Directory.GetFiles(_dicomStorePath, "*.dcm", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                DicomFile? dicomFile = null;
                try
                {
                    dicomFile = DicomFile.Open(file);
                }
                catch
                {
                    continue;
                }

                if (dicomFile != null)
                    yield return dicomFile.Dataset;
            }
        }

        /// <summary>
        /// Lấy DicomFile đầy đủ (bao gồm cả pixel data) theo UID
        /// </summary>
        public DicomFile? GetDicomFile(string studyInstanceUID, string seriesInstanceUID, string sopInstanceUID)
        {
            var files = Directory.GetFiles(_dicomStorePath, "*.dcm", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                try
                {
                    var dicom = DicomFile.Open(file);
                    var dataset = dicom.Dataset;

                    if (dataset.TryGetSingleValue(DicomTag.StudyInstanceUID, out string studyUID) &&
                        dataset.TryGetSingleValue(DicomTag.SeriesInstanceUID, out string seriesUID) &&
                        dataset.TryGetSingleValue(DicomTag.SOPInstanceUID, out string sopUID))
                    {
                        if (studyUID == studyInstanceUID &&
                            seriesUID == seriesInstanceUID &&
                            sopUID == sopInstanceUID)
                        {
                            return dicom;
                        }
                    }
                }
                catch
                {
                    continue;
                }
            }

            return null;
        }

        /// <summary>
        /// Chỉ lấy metadata (bỏ qua pixel data)
        /// </summary>
        public DicomFile? GetDicomFileMetadataOnly(string studyInstanceUID, string seriesInstanceUID)
        {
            var files = Directory.GetFiles(_dicomStorePath, "*.dcm", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                try
                {
                    var dicom = DicomFile.Open(file, FileReadOption.SkipLargeTags);
                    var dataset = dicom.Dataset;

                    if (dataset.TryGetSingleValue(DicomTag.StudyInstanceUID, out string studyUID) &&
                        dataset.TryGetSingleValue(DicomTag.SeriesInstanceUID, out string seriesUID) &&
                        dataset.TryGetSingleValue(DicomTag.SOPInstanceUID, out string sopUID))
                    {
                        if (studyUID == studyInstanceUID && seriesUID == seriesInstanceUID)
                            return dicom;
                    }
                }
                catch
                {
                    continue;
                }
            }

            return null;
        }

        /// <summary>
        /// Lấy danh sách các Series theo StudyInstanceUID
        /// </summary>
        public IEnumerable<DicomSerieModel> GetSeriesByStudyUID(string studyUID)
        {
            var seriesList = new List<DicomSerieModel>();
            var files = Directory.GetFiles(_dicomStorePath, "*.dcm", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                try
                {
                    var dicomFile = DicomFile.Open(file, FileReadOption.SkipLargeTags);
                    var dataset = dicomFile.Dataset;

                    if (dataset.GetSingleValueOrDefault(DicomTag.StudyInstanceUID, string.Empty) != studyUID)
                        continue;

                    var seriesUID = dataset.GetSingleValueOrDefault(DicomTag.SeriesInstanceUID, string.Empty);

                    if (seriesList.Any(s => s.SeriesInstanceUID == seriesUID))
                        continue; // Bỏ qua nếu đã có rồi (tránh trùng lặp)

                    var series = new DicomSerieModel
                    {
                        SeriesInstanceUID = seriesUID,
                        SeriesDescription = dataset.GetSingleValueOrDefault(DicomTag.SeriesDescription, string.Empty),
                        Modality = dataset.GetSingleValueOrDefault(DicomTag.Modality, string.Empty),
                        StudyInstanceUID = studyUID,
                        //SeriesDate = ParseDicomDate(dataset.GetSingleValueOrDefault(DicomTag.StudyDate, string.Empty)),
                        //AccessionNumber = dataset.GetSingleValueOrDefault(DicomTag.AccessionNumber, string.Empty)
                    };

                    seriesList.Add(series);
                }
                catch
                {
                    continue;
                }
            }

            return seriesList;
        }

        /// <summary>
        /// Hàm hỗ trợ parse DICOM date (yyyyMMdd) sang DateTime
        /// </summary>
        private DateTime ParseDicomDate(string dicomDate)
        {
            if (DateTime.TryParseExact(dicomDate, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out var date))
                return date;

            return DateTime.MinValue;
        }

        public async Task<List<DicomDataset>> GetAllStudiesAsync()
        {
            var result = new List<DicomDataset>();
            var files = Directory.GetFiles(_dicomStorePath, "*.dcm", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                try
                {
                    var dicomFile = await DicomFile.OpenAsync(file);
                    result.Add(dicomFile.Dataset);
                }
                catch
                {
                    continue;
                }
            }

            return result;
        }

        public async Task<DicomFile?> GetDicomFileAsync(string studyInstanceUID, string seriesInstanceUID, string sopInstanceUID)
        {
            var files = Directory.GetFiles(_dicomStorePath, "*.dcm", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                try
                {
                    var dicom = await DicomFile.OpenAsync(file);
                    var dataset = dicom.Dataset;

                    if (dataset.TryGetSingleValue(DicomTag.StudyInstanceUID, out string studyUID) &&
                        dataset.TryGetSingleValue(DicomTag.SeriesInstanceUID, out string seriesUID) &&
                        dataset.TryGetSingleValue(DicomTag.SOPInstanceUID, out string sopUID))
                    {
                        if (studyUID == studyInstanceUID &&
                            seriesUID == seriesInstanceUID &&
                            sopUID == sopInstanceUID)
                        {
                            return dicom;
                        }
                    }
                }
                catch
                {
                    continue;
                }
            }

            return null;
        }

        public async Task<DicomFile?> GetDicomFileMetadataOnlyAsync(string studyInstanceUID, string seriesInstanceUID)
        {
            var files = Directory.GetFiles(_dicomStorePath, "*.dcm", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                try
                {
                    var dicom = await DicomFile.OpenAsync(file, FileReadOption.SkipLargeTags);
                    var dataset = dicom.Dataset;

                    if (dataset.TryGetSingleValue(DicomTag.StudyInstanceUID, out string studyUID) &&
                        dataset.TryGetSingleValue(DicomTag.SeriesInstanceUID, out string seriesUID) &&
                        dataset.TryGetSingleValue(DicomTag.SOPInstanceUID, out string sopUID))
                    {
                        if (studyUID == studyInstanceUID && seriesUID == seriesInstanceUID)
                            return dicom;
                    }
                }
                catch
                {
                    continue;
                }
            }

            return null;
        }
    }
}
