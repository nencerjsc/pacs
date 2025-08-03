using Microsoft.EntityFrameworkCore;
using NencerApi.Modules.PacsServer.Model;
using NencerCore;
using System.Globalization;

namespace NencerApi.Modules.PacsServer.Service
{
    public class DicomWorkListService
    {
        private readonly AppDbContext _context;

        public DicomWorkListService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<DicomWorkListModel>> GetAllAsync() => await _context.DicomWorkLists.ToListAsync();

        public async Task<bool> CheckIfWorkListExists(string orderService, string accessionNumber)
        {
            var existingWorkList = await _context.DicomWorkLists
                .FirstOrDefaultAsync(w => w.OrderRequestId == orderService && w.AccessionNumber == accessionNumber);

            return existingWorkList != null;
        }

        public async Task<List<DicomWorkListModel>> GetWorkListAsync(
            string patientName = "",
            string doctorName = "",
            DateTime? startDate = null,
            DateTime? endDate = null,
            string statusString = "")
        {
            var query = _context.DicomWorkLists.AsQueryable();

            if (!string.IsNullOrEmpty(patientName))
            {
                query = query.Where(w => w.PatientName.Contains(patientName));
            }

            if (!string.IsNullOrEmpty(doctorName))
            {
                query = query.Where(w => w.ReferringPhysician.Contains(doctorName));
            }

            if (startDate.HasValue)
            {
                query = query.Where(w => DateTime.ParseExact(w.ScheduledProcedureStepStartDate, "yyyyMMdd", CultureInfo.InvariantCulture) >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(w => DateTime.ParseExact(w.ScheduledProcedureStepStartDate, "yyyyMMdd", CultureInfo.InvariantCulture) <= endDate.Value);
            }

            if (!string.IsNullOrEmpty(statusString))
            {
                WorklistStatus status = (WorklistStatus)Enum.Parse(typeof(WorklistStatus), statusString);
                query = query.Where(w => w.Status == status);
            }

            return await query.ToListAsync();
        }

        public async Task<string> GetOrderRequestIdByAccessionNumberAsync(string accessionNumber)
        {
            var workList = await _context.DicomWorkLists
                .Where(w => w.AccessionNumber == accessionNumber)
                .FirstOrDefaultAsync();
            if (workList == null)
            {
                return "";
            }
            return workList.OrderRequestId;
        }

        // Create a new WorkList asynchronously
        public async Task<DicomWorkListModel> CreateAsync(DicomWorkListModel model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.AccessionNumber)) model.AccessionNumber = GenerateAccessionNumber();
                _context.DicomWorkLists.Add(model);
                await _context.SaveChangesAsync();

                return model;
            }
            catch (Exception ex)
            {
                throw new Exception("Error creating worklist: " + ex.Message, ex);
            }
        }

        public async Task<bool> UpdateAsync(string accessionNumber, DicomWorkListModel model)
        {
            return false;
        }

        public async Task<bool> DeleteAsync(string accessionNumber)
        {
            return false;
        }

        // Hàm tạo AccessionNumber tối ưu
        public string GenerateAccessionNumber()
        {
            var currentDate = DateTime.Now.ToString("yyyyMMdd");
            var prefix = "NC" + currentDate;

            var maxNumber = _context.DicomWorkLists
                .Where(w => w.AccessionNumber.StartsWith(prefix))
                .AsEnumerable()
                .Select(w =>
                {
                    var numberPart = w.AccessionNumber.Substring(prefix.Length); // lấy phần số sau prefix
                    return int.TryParse(numberPart, out int n) ? n : 0;
                })
                .DefaultIfEmpty(0)
                .Max();

            var newNumber = maxNumber + 1;

            // Tạo AccessionNumber 'NCyyyyMMddxxx' (xxx: 6 số)
            return $"{prefix}{newNumber:D6}";
        }
    }
}
