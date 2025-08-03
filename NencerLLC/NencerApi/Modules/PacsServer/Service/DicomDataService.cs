using Microsoft.EntityFrameworkCore;
using NencerApi.Modules.PacsServer.Model;
using NencerCore;

namespace NencerApi.Modules.PacsServer.Service
{
    public class DicomDataService
    {
        private readonly AppDbContext _context;

        public DicomDataService(AppDbContext context, IConfiguration config)
        {
            _context = context;
        }

        public async Task<List<DicomStudyModel>> GetAllStudiesAsync()
        {
            return await _context.DicomStudies.ToListAsync();
        }
        public async Task<DicomStudyModel?> GetStudyByIdAsync(string studyInstanceUID)
        {
            return await _context.DicomStudies.FindAsync(studyInstanceUID);
        }
        public async Task<bool> CheckExistsStudyInstanceUIDAsync(string studyInstanceUID)
        {
            return await _context.DicomStudies.AnyAsync(x => x.StudyInstanceUID == studyInstanceUID);
        }
        public async Task<bool> CreateStudyAsync(DicomStudyModel model)
        {
            try
            {
                _context.DicomStudies.Add(model);
                int result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    //Log.Information("Đã thêm DicomStudy thành công. StudyInstanceUID: {StudyInstanceUID}", model.StudyInstanceUID);
                    return true;
                }
                else
                {
                    //Log.Warning("Thêm DicomStudy không thành công. Không có bản ghi nào được lưu.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                //Log.Error(ex, "Lỗi khi thêm DicomStudy. StudyInstanceUID: {StudyInstanceUID}", model.StudyInstanceUID);
                return false;
            }
        }

        public async Task<bool> UpdateStudyAsync(DicomStudyModel updatedModel)
        {
            var existing = await _context.DicomStudies
                .FirstOrDefaultAsync(s => s.StudyInstanceUID == updatedModel.StudyInstanceUID);

            if (existing == null)
                return false;

            // Cập nhật các trường cần thiết (ở đây là số lượng series và instance)
            existing.NumberOfStudyRelatedSeries = updatedModel.NumberOfStudyRelatedSeries;
            existing.NumberOfStudyRelatedInstances = updatedModel.NumberOfStudyRelatedInstances;

            _context.DicomStudies.Update(existing);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<DicomStudyModel?> GetStudyByInstanceUIDAsync(string studyInstanceUID)
        {
            if (string.IsNullOrWhiteSpace(studyInstanceUID))
                return null;

            return await _context.DicomStudies
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.StudyInstanceUID == studyInstanceUID);
        }

        // Series
        public async Task<bool> CheckExistsSerieInstanceUIDAsync(string seriesInstanceUID)
        {
            return await _context.DicomSeries.AnyAsync(x => x.SeriesInstanceUID == seriesInstanceUID);
        }

        public async Task<DicomSerieModel?> GetSeriesByInstanceUIDAsync(string seriesInstanceUID)
        {
            return await _context.DicomSeries
                .FirstOrDefaultAsync(s => s.SeriesInstanceUID == seriesInstanceUID);
        }

        public async Task<bool> UpdateSeriesAsync(DicomSerieModel model)
        {
            _context.DicomSeries.Update(model);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> CreateSerieAsync(DicomSerieModel model)
        {
            try
            {
                _context.DicomSeries.Add(model);
                int result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    //Log.Information("Đã thêm DicomStudy thành công. StudyInstanceUID: {StudyInstanceUID}", model.StudyInstanceUID);
                    return true;
                }
                else
                {
                    //Log.Warning("Thêm DicomStudy không thành công. Không có bản ghi nào được lưu.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                //Log.Error(ex, "Lỗi khi thêm DicomStudy. StudyInstanceUID: {StudyInstanceUID}", model.StudyInstanceUID);
                return false;
            }
        }

        // Instance

        public async Task<bool> CheckExistsSOPInstanceUIDAsync(string sopInstanceUID)
        {
            return await _context.DicomInstances.AnyAsync(x => x.SOPInstanceUID == sopInstanceUID);
        }

        public async Task<bool> CreateSOPInstanceAsync(DicomInstanceModel model)
        {
            try
            {
                _context.DicomInstances.Add(model);
                int result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    //Log.Information("Đã thêm DicomStudy thành công. StudyInstanceUID: {StudyInstanceUID}", model.StudyInstanceUID);
                    return true;
                }
                else
                {
                    //Log.Warning("Thêm DicomStudy không thành công. Không có bản ghi nào được lưu.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                //Log.Error(ex, "Lỗi khi thêm DicomStudy. StudyInstanceUID: {StudyInstanceUID}", model.StudyInstanceUID);
                return false;
            }
        }

        public async Task SaveDicomTagsAsync(string sopInstanceUID, List<DicomTagItemModel> tags)
        {
            if (tags == null || !tags.Any())
                return;

            // Set lại SOPInstanceUID cho chắc chắn
            foreach (var tag in tags)
            {
                tag.SOPInstanceUID = sopInstanceUID;
            }

            // Bước 1: Lấy danh sách Name đã tồn tại trong database
            var existingTagNames = await _context.DicomTags
                .Where(x => x.SOPInstanceUID == sopInstanceUID)
                .Select(x => x.Name)
                .ToListAsync();

            // Bước 2: Lọc bỏ những tag đã tồn tại (theo Name)
            var newTags = tags
                .Where(tag => !existingTagNames.Contains(tag.Name))
                .ToList();

            if (newTags.Any())
            {
                // Bước 3: Insert các tag chưa có
                await _context.DicomTags.AddRangeAsync(newTags);
                await _context.SaveChangesAsync();
            }
        }

        // WorkList
        public async Task<List<DicomWorkListModel>> GetAllWorkListAsync() => await _context.DicomWorkLists.ToListAsync();

        public async Task<DicomWorkListModel?> GetWorkListByAccessionNumberAsync(string accessionNumber) =>
            await _context.DicomWorkLists.FirstOrDefaultAsync(x => x.AccessionNumber == accessionNumber);

        public async Task CreateAsync(DicomWorkListModel model)
        {
            _context.DicomWorkLists.Add(model);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> UpdateWorkListAsync(string accessionNumber, DicomWorkListModel model)
        {
            var existing = await GetWorkListByAccessionNumberAsync(accessionNumber);
            if (existing == null) return false;

            // Gán lại từng thuộc tính bạn muốn cập nhật
            existing.Modality = model.Modality;
            existing.PatientName = model.PatientName;
            existing.StudyDate = model.StudyDate;
            existing.Status = model.Status;
            // ...

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteWorkListAsync(string accessionNumber)
        {
            var existing = await GetWorkListByAccessionNumberAsync(accessionNumber);
            if (existing == null) return false;

            _context.DicomWorkLists.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
