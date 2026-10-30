using OklahomaRentals.Server.Data;
using Microsoft.EntityFrameworkCore;
using OklahomaRentals.Server.Services.Interfaces;
using OklahomaRentals.Server.Services.Implementations;
using OklahomaRentals.Server.Repositories.Interfaces;  
using OklahomaRentals.Server.Repositories.Implementations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

/**** TEMP WORK AROUND FOR TESTING *****/
builder.Services.AddDbContext<ORDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 0))
    ));

//builder.Services.AddDbContext<ORDbContext>(options =>
//    options.UseMySql(
//        builder.Configuration.GetConnectionString("DefaultConnection"),
//        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
//    ));

builder.Services.AddScoped<IPropertyRepository, PropertyRepository>();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// register services here, so they can be used in controllers 
builder.Services.AddScoped<IPropertyService, PropertyService>();
builder.Services.AddScoped<IMaintenanceService, MaintenanceService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAdminRepository, AdminRepository>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<ICloudinaryService, CloudinaryService>();

builder.Services.AddScoped<IApplicationRepository, ApplicationRepository>();
builder.Services.AddScoped<IApplicationService, ApplicationService>();

builder.Services.AddScoped<IDocumentRepository, DocumentRepository>();
builder.Services.AddScoped<IDocumentService, DocumentService>();

builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IPaymentService, PaymentService>();

builder.Services.AddScoped<ITenantRepository, TenantRepository>();
builder.Services.AddScoped<ITenantService, TenantService>();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddScoped<IMaintenanceRequestRepository, MaintenanceRequestRepository>();

builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IApplicationEmailService, ApplicationEmailService>();

// The public application form sends email, so cap submissions per visitor.
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddPolicy("applications", context => RateLimitPartition.GetFixedWindowLimiter(
        context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
        _ => new FixedWindowRateLimiterOptions { PermitLimit = 5, Window = TimeSpan.FromMinutes(15) }));
});

// Add CORS for React frontend port 52106
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        policy => policy.WithOrigins("http://localhost:52106", "https://localhost:52106")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials());
});

// Keycloak JWT authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.Authority = builder.Configuration["Keycloak:Authority"];
    options.Audience = builder.Configuration["Keycloak:Audience"];
    options.RequireHttpsMetadata = false; // dev only, set true in prod
});

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseDefaultFiles();
app.MapStaticAssets();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}


app.UseHttpsRedirection();

app.UseCors("AllowReactApp");

app.UseAuthentication();

app.UseAuthorization();

app.UseRateLimiter();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();
