using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using FellowOakDicom;
using log4net;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NencerApi.ActionFilter;
using NencerApi.Extentions;
using NencerApi.Helpers;
using NencerApi.Modules.PacsServer.Server;
using NencerApi.Modules.SystemNc.Service;
using NencerApi.Modules.User.Model;
using NencerApi.Modules.User.Service;
using NencerCore;
using NuGet.Protocol;
using Serilog.Events;
using Serilog;
using System;
using System.Net.WebSockets;
using System.Text;
using NencerApi.Modules.PacsServer.Service;
using NencerApi.Modules.PacsServer.Config;

new DicomSetupBuilder()
    .RegisterServices(s => s.AddFellowOakDicom().AddLogging(config => config.AddSerilog()))
    .Build();

var builder = WebApplication.CreateBuilder(args);

// Cấu hình Serilog
Log.Logger = new LoggerConfiguration()
     .MinimumLevel.Debug()
     .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
     .MinimumLevel.Override("System", LogEventLevel.Warning)
    .WriteTo.Console()
    .WriteTo.File(
        path: "logs/log-.txt", // Đường dẫn thư mục và tên file (logs/log-[ngày].txt)
        rollingInterval: RollingInterval.Day, // Tạo file mới mỗi ngày
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}",
         fileSizeLimitBytes: 50_000_000,
         rollOnFileSizeLimit: true
    )
    .CreateLogger();

// Thêm cấu hình JSON
builder.Configuration.SetBasePath(Directory.GetCurrentDirectory())
                     .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                     .AddJsonFile("appsettings.custom.json", optional: true, reloadOnChange: true);

// Cấu hình CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy
            .AllowAnyOrigin()    // Cho phép tất cả origin
            .AllowAnyHeader()    // Cho phép mọi header
            .AllowAnyMethod();   // Cho phép mọi HTTP method (GET, POST, …)
                                 // Không gọi .AllowCredentials()
    });
});


// Cấu hình DbContext sử dụng SQL Server
builder.Services.AddDbContext<AppDbContext>();

// Cấu hình các dịch vụ khác
builder.Services.AddControllers(opt =>
{
    opt.Filters.Add(typeof(CustomExceptionFilter));
    opt.Filters.Add<LogActionFilter>();
});


// Đăng ký Hosted Service
//Lưu ý khi chạy trên IIS
//IIS sẽ recycle AppPool nếu không có request trong một thời gian, làm listener dừng.
//→ Vào IIS > AppPool > Advanced Settings:
//Idle Time-out (minutes) → đặt thành 0.
//Start Mode → AlwaysRunning.
//Vào IIS > Website > Advanced Settings > Preload Enabled → True.
//Bật Application Initialization Module để App tự khởi động khi IIS restart.
builder.Services.AddHostedService<DicomListenerHostedService>();
builder.Services.AddSingleton<DicomListenerManager>();

// Cấu hình log4net
builder.Services.AddSingleton<ILog>(LogManager.GetLogger(typeof(Program)));
builder.Services.AddMemoryCache();


// Đăng ký các service
builder.Services.AddScoped<NencerApi.Helpers.UserService>();
builder.Services.AddSingleton<JwtService>();
builder.Services.AddScoped<RoleService>();
builder.Services.AddScoped<PermissionService>();
builder.Services.AddScoped<NencerApi.Modules.User.Service.UserService>();
builder.Services.AddScoped<SettingService>();
builder.Services.AddScoped<SendmessService>();
builder.Services.AddScoped<CheckPermissionService>();
builder.Services.AddScoped<Statistics>();
builder.Services.AddScoped<DicomWorkListService>();


// Cấu hình xác thực JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        var secretKey = builder.Configuration["Jwt:Key"];
        opt.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

// Thêm Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "NencerAPI", Version = "v1" });
    opt.OperationFilter<AddAcceptLanguageHeaderParameter>();
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });
    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[]{ }
        }
    });
});

// Đăng ký AutoMapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Thêm dịch vụ WebSocketHandler
builder.Services.AddSingleton<WebSocketHandler>();

var app = builder.Build();

var listener = app.Services.GetRequiredService<DicomListenerManager>();
listener.Start();

// Áp dụng CORS
app.UseCors("CorsPolicy");

// Cấu hình pipeline xử lý request

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthentication(); // Đảm bảo UseAuthentication trước UseAuthorization
app.UseAuthorization();
app.UseResponseCaching();
//app.Urls.Add("http://0.0.0.0:5002"); // Lắng nghe trên cổng 5000
app.MapControllers();

//Thêm websocket
app.UseWebSockets();
app.Map("/ws", async context =>
{
    var webSocketHandler = app.Services.GetRequiredService<WebSocketHandler>();
    await webSocketHandler.HandleAsync(context);
});


using (var scope = app.Services.CreateScope())
{
    var settingService = scope.ServiceProvider.GetRequiredService<SettingService>();

    // Gọi ValidateLicense
    if (!settingService.ValidateLicense())
    {
        //Environment.Exit(1); // Thoát chương trình nếu license không hợp lệ
    }
}

app.Run();
