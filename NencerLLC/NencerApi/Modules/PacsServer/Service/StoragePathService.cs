using Microsoft.EntityFrameworkCore;
using NencerApi.Modules.PacsServer.Model;
using NencerCore;

namespace NencerApi.Modules.PacsServer.Service
{
    public class StoragePathService
    {
        private readonly AppDbContext _context;

        public StoragePathService()
        {
            _context = new AppDbContext();
        }

        public async Task ActivateStorageAsync(int storageId)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                await _context.StoragePaths
                    .Where(s => s.IsActive)
                    .ExecuteUpdateAsync(s => s.SetProperty(x => x.IsActive, false));

                await _context.StoragePaths
                    .Where(s => s.Id == storageId)
                    .ExecuteUpdateAsync(s => s.SetProperty(x => x.IsActive, true));

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public Task<StoragePathModel?> GetActiveStorageAsync()
            => _context.StoragePaths.FirstOrDefaultAsync(s => s.IsActive);

        public Task<List<StoragePathModel>> GetAllAsync()
            => _context.StoragePaths.OrderBy(x => x.Priority).ToListAsync();

        public Task<StoragePathModel?> GetByIdAsync(int id)
            => _context.StoragePaths.FindAsync(id).AsTask();

        public async Task<StoragePathModel> AddAsync(StoragePathModel model)
        {
            _context.StoragePaths.Add(model);
            await _context.SaveChangesAsync();
            return model;
        }

        public async Task UpdateAsync(StoragePathModel model)
        {
            var existing = await _context.StoragePaths.FindAsync(model.Id);
            if (existing != null)
            {
                existing.Path = model.Path;
                existing.Priority = model.Priority;
                existing.IsActive = model.IsActive;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<StoragePathModel?> DeleteAsync(int id)
        {
            var model = await _context.StoragePaths.FindAsync(id);
            if (model != null)
            {
                _context.StoragePaths.Remove(model);
                await _context.SaveChangesAsync();
                return model;
            }
            return null;
        }

        public async Task<StoragePathModel?> SetPriorityAsync(int id, int priority)
        {
            var model = await _context.StoragePaths.FindAsync(id);
            if (model != null)
            {
                model.Priority = priority;
                await _context.SaveChangesAsync();
                return model;
            }
            return null;
        }

        public Task DeactivateAllAsync()
            => _context.StoragePaths.Where(s => s.IsActive)
                .ExecuteUpdateAsync(s => s.SetProperty(x => x.IsActive, false));
    }
}
