using Microsoft.Extensions.Caching.Memory;
using NencerCore;
using Microsoft.Extensions.Caching.Memory;
using NencerApi.Helpers;
using Microsoft.EntityFrameworkCore;
namespace NencerApi.Modules.User.Service
{
    public class UserService
    {
        private readonly AppDbContext _context;
        private readonly IMemoryCache _memoryCache;
        public UserService(AppDbContext context, IMemoryCache memoryCache)
        {
            _context = context;
            _memoryCache = memoryCache;
        }
        public async Task<string> Getname (int Id)
        {
            string userName = null;
            if (Id > 0)
            {
                string cacheKey = $"User_{Id}";
                if (!_memoryCache.TryGetValue(cacheKey, out userName))
                {
                    var user = _context.Users.FirstOrDefault(x => x.Id == Id);
                    if (user != null)
                    {
                        userName = user.Name;
                        // Lưu vào cache với thời gian hết hạn (Expiration)
                        var cacheEntryOptions = new MemoryCacheEntryOptions()
                        .SetSlidingExpiration(TimeSpan.FromMinutes(30)); // Expire sau 30 phút không sử dụng
                        _memoryCache.Set(cacheKey, userName, cacheEntryOptions);
                    }

                }
            }
            return userName;
        }

        public async Task<string?> GetSignByUserId(int userId)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(x=> x.Id == userId);
                if (user == null)
                {
                    return null;
                }
                return user.Signature;
            }
            catch (Exception ex)
            {
                LogHelper.Exception("GetSignByUsername", ex);
                return null;
            }
        }
    }
}
