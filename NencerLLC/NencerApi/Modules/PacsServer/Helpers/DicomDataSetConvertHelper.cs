using FellowOakDicom;
using NencerApi.Modules.PacsServer.Model;

namespace NencerApi.Modules.PacsServer.Helpers
{
    public class DicomDataSetConvertHelper
    {
        private DicomDataset _dataset;
        public DicomDataSetConvertHelper(DicomDataset ds)
        {
            _dataset = ds;
        }
        public DicomStudyModel ConvertToStudyModel()
        {
            return new DicomStudyModel
            {
                StudyInstanceUID = _dataset.GetSingleValueOrDefault(DicomTag.StudyInstanceUID, string.Empty),
                StudyDate = _dataset.GetSingleValueOrDefault(DicomTag.StudyDate, string.Empty),
                StudyTime = _dataset.GetSingleValueOrDefault(DicomTag.StudyTime, string.Empty),
                AccessionNumber = _dataset.GetSingleValueOrDefault(DicomTag.AccessionNumber, string.Empty),
                StudyDescription = _dataset.GetSingleValueOrDefault(DicomTag.StudyDescription, string.Empty),
                PatientID = _dataset.GetSingleValueOrDefault(DicomTag.PatientID, string.Empty),
                PatientName = _dataset.GetSingleValueOrDefault(DicomTag.PatientName, string.Empty),
                PatientBirthDate = _dataset.GetSingleValueOrDefault(DicomTag.PatientBirthDate, string.Empty),
                PatientSex = _dataset.GetSingleValueOrDefault(DicomTag.PatientSex, string.Empty),

                NumberOfStudyRelatedSeries = _dataset.GetSingleValueOrDefault(DicomTag.NumberOfStudyRelatedSeries, 0),
                NumberOfStudyRelatedInstances = _dataset.GetSingleValueOrDefault(DicomTag.NumberOfStudyRelatedInstances, 0),
            };
        }
        public DicomSerieModel ConvertToSerieModel()
        {
            return new DicomSerieModel
            {
                StudyInstanceUID = _dataset.GetSingleValueOrDefault(DicomTag.StudyInstanceUID, string.Empty),
                SeriesInstanceUID = _dataset.GetSingleValueOrDefault(DicomTag.SeriesInstanceUID, string.Empty),
                Modality = _dataset.GetSingleValueOrDefault(DicomTag.Modality, string.Empty),
                SeriesNumber = _dataset.GetSingleValueOrDefault(DicomTag.SeriesNumber, 0),
                SeriesDescription = _dataset.GetSingleValueOrDefault(DicomTag.SeriesDescription, string.Empty)
            };
        }
        public DicomInstanceModel ConvertToInstanceModel(string dicomFilePathLocal)
        {
            return new DicomInstanceModel
            {
                PatientId = _dataset.GetSingleValueOrDefault(DicomTag.PatientID, string.Empty),
                SOPInstanceUID = _dataset.GetSingleValueOrDefault(DicomTag.SOPInstanceUID, string.Empty),
                StudyInstanceUID = _dataset.GetSingleValueOrDefault(DicomTag.StudyInstanceUID, string.Empty),
                SOPClassUID = _dataset.GetSingleValueOrDefault(DicomTag.SOPClassUID, string.Empty),
                SeriesInstanceUID = _dataset.GetSingleValueOrDefault(DicomTag.SeriesInstanceUID, string.Empty),
                Modality = _dataset.GetSingleValueOrDefault(DicomTag.Modality, string.Empty),
                SeriesNumber = _dataset.GetSingleValueOrDefault(DicomTag.SeriesNumber, 0),
                InstanceNumber = _dataset.GetSingleValueOrDefault(DicomTag.InstanceNumber, 0),
                ImageType = _dataset.GetSingleValueOrDefault(DicomTag.ImageType, string.Empty),

                AcquisitionDate = _dataset.GetSingleValueOrDefault(DicomTag.AcquisitionDate, string.Empty),
                AcquisitionTime = _dataset.GetSingleValueOrDefault(DicomTag.AcquisitionTime, string.Empty),
                ContentDate = _dataset.GetSingleValueOrDefault(DicomTag.ContentDate, string.Empty),
                ContentTime = _dataset.GetSingleValueOrDefault(DicomTag.ContentTime, string.Empty),
                ImagePositionPatient = _dataset.GetSingleValueOrDefault(DicomTag.ImagePositionPatient, string.Empty),
                ImageOrientationPatient = _dataset.GetSingleValueOrDefault(DicomTag.ImageOrientationPatient, string.Empty),

                RowsAndColumns = new[]
                {
                    _dataset.GetSingleValueOrDefault(DicomTag.Rows, 0),
                    _dataset.GetSingleValueOrDefault(DicomTag.Columns, 0)
                },

                PhotometricInterpretation = _dataset.GetSingleValueOrDefault(DicomTag.PhotometricInterpretation, string.Empty),
                WindowCenter = _dataset.TryGetSingleValue(DicomTag.WindowCenter, out float wc) ? wc : null,
                WindowWidth = _dataset.TryGetSingleValue(DicomTag.WindowWidth, out float ww) ? ww : null,

                SliceLocation = _dataset.GetSingleValueOrDefault(DicomTag.SliceLocation, string.Empty),
                PixelSpacing = _dataset.GetSingleValueOrDefault(DicomTag.PixelSpacing, string.Empty),

                BitsAllocated = _dataset.GetSingleValueOrDefault(DicomTag.BitsAllocated, (ushort)0),
                BitsStored = _dataset.GetSingleValueOrDefault(DicomTag.BitsStored, (ushort)0),
                HighBit = _dataset.GetSingleValueOrDefault(DicomTag.HighBit, (ushort)0),
                RescaleIntercept = _dataset.GetSingleValueOrDefault(DicomTag.RescaleIntercept, string.Empty),
                RescaleSlope = _dataset.GetSingleValueOrDefault(DicomTag.RescaleSlope, string.Empty),

                TransferSyntaxUID = _dataset.InternalTransferSyntax?.UID.UID ?? string.Empty,
                FilePath = dicomFilePathLocal,
                CreatedAt = DateTime.UtcNow
            };
        }

        public PacsDicomResultModel ConvertToPacsDicomResultModel()
        {
            return new PacsDicomResultModel
            {
                // Thông tin bệnh nhân
                PatientId = _dataset.GetSingleValueOrDefault(DicomTag.PatientID, string.Empty),
                PatientName = _dataset.GetSingleValueOrDefault(DicomTag.PatientName, string.Empty),
                PatientBirthDate = _dataset.GetSingleValueOrDefault(DicomTag.PatientBirthDate, string.Empty),
                PatientGender = _dataset.GetSingleValueOrDefault(DicomTag.PatientSex, string.Empty),

                // Thông tin hình ảnh
                StudyId = _dataset.GetSingleValueOrDefault(DicomTag.StudyID, string.Empty),
                StudyAccessionNumber = _dataset.GetSingleValueOrDefault(DicomTag.AccessionNumber, string.Empty),
                StudyInstanceUID = _dataset.GetSingleValueOrDefault(DicomTag.StudyInstanceUID, string.Empty),
                StudySeriesInstanceUID = _dataset.GetSingleValueOrDefault(DicomTag.SeriesInstanceUID, string.Empty),
                StudySOPInstanceUID = _dataset.GetSingleValueOrDefault(DicomTag.SOPInstanceUID, string.Empty),

                StudyStarted = TryParseDateTimeOffset(
                    _dataset.GetSingleValueOrDefault(DicomTag.StudyDate, string.Empty),
                    _dataset.GetSingleValueOrDefault(DicomTag.StudyTime, string.Empty)
                ),

                // Các trường yêu cầu dịch vụ có thể gán sau (tùy thuộc logic của bạn)
                ServiceRequestId = null,
                ServiceRequestAccessionNumber = null,
                ServiceRequestStatus = null,
                ServiceRequestIntent = null,
                ServiceRequestCode = null,
                ServiceRequestRequester = null,
                ServiceRequestPatient = null,
                ServiceRequestReason = null,
                ServiceRequestAuthoredOn = null,

                // Endpoint PACS có thể gán sau nếu cần
                EndpointId = null,
                EndpointAddress = null,
                EndpointConnectionCode = null,
                EndpointConnectionDisplay = null,

                CreatedAt = DateTime.UtcNow
            };
        }

        public List<DicomTagItemModel> ConvertToDicomTagItemModelList()
        {
            var tags = new List<DicomTagItemModel>();

            // Lấy trước SOPInstanceUID từ Dataset (dùng cho PixelData hoặc gán vào từng tag)
            var sopInstanceUID = _dataset.GetSingleValueOrDefault<string>(DicomTag.SOPInstanceUID, "");

            // Hàm đệ quy xử lý Dataset
            void ParseDataset(DicomDataset dataset, string parentTag = "", int? groupId = null)
            {
                foreach (var item in dataset)
                {
                    try
                    {
                        var tag = item.Tag;
                        string name = $"{tag.Group:X4}{tag.Element:X4}";
                        string title = DicomDictionary.Default[tag]?.Name ?? string.Empty;
                        string valueType = item.ValueRepresentation?.Code ?? string.Empty;

                        if (valueType.Equals("SQ", StringComparison.OrdinalIgnoreCase))
                        {
                            // Nếu là Sequence
                            var sequenceItems = dataset.GetSequence(tag);
                            int itemIndex = 0;
                            foreach (var sequenceDataset in sequenceItems)
                            {
                                // Đệ quy vào bên trong từng dataset con
                                ParseDataset(sequenceDataset, parentTag: name, groupId: itemIndex);
                                itemIndex++;
                            }

                            // Lưu tag cha của SQ
                            tags.Add(new DicomTagItemModel
                            {
                                SOPInstanceUID = sopInstanceUID,
                                Name = name,
                                Title = title,
                                ValueType = valueType,
                                Value = string.Empty, // SQ không có Value trực tiếp
                                ParentTag = parentTag,
                                GroupId = groupId
                            });
                        }
                        else
                        {
                            // Các tag bình thường
                            string value = string.Empty;

                            if (name.Equals("7FE00010", StringComparison.OrdinalIgnoreCase))
                            {
                                // Pixel Data đặc biệt
                                value = $"instances/{sopInstanceUID}/frames";
                            }
                            else
                            {
                                if (dataset.TryGetString(tag, out var result))
                                {
                                    value = result;
                                }
                            }

                            tags.Add(new DicomTagItemModel
                            {
                                SOPInstanceUID = sopInstanceUID,
                                Name = name,
                                Title = title,
                                ValueType = valueType,
                                Value = value,
                                ParentTag = parentTag,
                                GroupId = groupId
                            });
                        }
                    }
                    catch
                    {
                        // Nếu lỗi đọc 1 tag thì bỏ qua, tiếp tục
                        continue;
                    }
                }
            }

            // Gọi lần đầu tiên (root dataset)
            ParseDataset(_dataset);

            return tags;
        }


        private DateTimeOffset? TryParseDateTimeOffset(string date, string time)
        {
            if (DateTime.TryParseExact(
                    date + time,
                    new[] { "yyyyMMddHHmmss", "yyyyMMddHHmm", "yyyyMMdd" },
                    null,
                    System.Globalization.DateTimeStyles.AssumeUniversal,
                    out var dt))
            {
                return new DateTimeOffset(dt);
            }
            return null;
        }

    }
}
