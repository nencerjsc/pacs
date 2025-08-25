using Microsoft.EntityFrameworkCore;
using NencerApi.Modules.SystemNc.Model;
using NencerApi.Modules.User.Model;
using System.Data;
using NencerApi.Helpers;
using NencerApi.Modules.PacsServer.Model;
using NencerApi.Modules.PacsServer.Config;
using Serilog;
namespace NencerCore
{
    public class AppDbContext : DbContext
    {
        public DbSet<Setting> Settings { get; set; }
        public DbSet<WebData> WebDatas { get; set; }
        public DbSet<UserModel> User { get; set; } = default!;
        public DbSet<UserModel> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; } = default!;
        public DbSet<UserHasRoles> UserHasRoles { get; set; } = default!;
        public DbSet<UserRoom> UserRooms { get; set; }
        public DbSet<SendmessModel> Sendmesses { get; set; }
        public DbSet<MenuFirst> MenuFirsts { get; set; }
        public DbSet<MenuSecond> MenuSeconds { get; set; }
        public DbSet<DigiSignLog> DigiSignLogs { get; set; }
        public DbSet<DigiSignProvider> DigiSignProviders { get; set; }
        public DbSet<FileServer> FileServers { get; set; }

        public DbSet<CurrencyModel> CurrenciesModel { get; set; } = default!;

        public DbSet<UserDepartmentsModel> UserDepartmentsModel { get; set; } = default!;
        public DbSet<UserJobReviewsModel> UserJobReviewsModel { get; set; } = default!;
        public DbSet<UserPositionsModel> UserPositionsModel { get; set; } = default!;

        public DbSet<DicomStudyModel> DicomStudies { get; set; } = default!;
        public DbSet<DicomInstanceModel> DicomInstances { get; set; } = default!;
        public DbSet<DicomSerieModel> DicomSeries { get; set; } = default!;
        public DbSet<DicomTagItemModel> DicomTags { get; set; } = default!;
        public DbSet<StoragePathModel> StoragePaths { get; set; }
        public DbSet<DicomWorkListModel> DicomWorkLists { get; set; }
        public DbSet<PacsDicomResultModel> PacsDicomResults { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            try
            {
                optionsBuilder.UseSqlServer(AppConfig.Database.SqlServerConnection);
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "⚠️ Lỗi khi cấu hình DbContext. Sử dụng cấu hình mặc định.");
            }

        }
    }

}
