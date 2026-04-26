using StockManager.Infrastructure.Persistence;
using StockManager.Infrastructure.Persistence.Repositories;
using StockManager.Infrastructure.Services;
using StockManager.Core.Application.Interfaces.Repositories;
using StockManager.Core.Application.Interfaces.Services;
using StockManager.Core.Application.Services;
using StockManager.Core.Application.Services.Catalogue;
using StockManager.Core.Application.Mapping;
using StockManager.API.Middleware;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using StockManager.Core.Domain.Entities;

var builder = WebApplication.CreateBuilder(args);

// Cloud Deployment: Listen on PORT environment variable if provided (e.g. Render)
var port = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrEmpty(port))
{
    builder.WebHost.UseUrls($"http://*:{port}");
}

// Add services to the container.
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Standardize Validation Error Response removed from here and moved to AddControllers

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var serverVersion = new MySqlServerVersion(new Version(8, 0, 0));

builder.Services.AddDbContext<StockManagementDbContext>(options =>
    options.UseMySql(connectionString, serverVersion,
        b => b.MigrationsAssembly("StockManager.Infrastructure")));

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var firstError = context.ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .FirstOrDefault() ?? "Les données fournies sont invalides.";

            var response = new 
            { 
                code = "VALIDATION_ERROR", 
                message = firstError 
            };

            return new BadRequestObjectResult(response);
        };
    })
    .AddJsonOptions(options =>
    {
        // Accept enum values as their string names (UPPERCASE, matching enum definitions)
        // This handles all enum DTOs including EquipmentCondition, MovementType, etc.
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });

// ── JWT Authentication ───────────────────────────────────────────────────
var key = Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"] ?? "CapLedSecretKey_ChangeInProduction_2024");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // Set to true in production
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// Register Repositories
builder.Services.AddScoped<IEquipmentRepository, EquipmentRepository>();
builder.Services.AddScoped<IStockMovementRepository, StockMovementRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IFamilleRepository, FamilleRepository>();
builder.Services.AddScoped<IContactRequestRepository, ContactRequestRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IStockQuantiteRepository, StockQuantiteRepository>();
builder.Services.AddScoped<IAlerteStockRepository, AlerteStockRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ILotRepository, LotRepository>();
builder.Services.AddScoped<INumeroSerieRepository, NumeroSerieRepository>();

// Register Services
builder.Services.AddScoped<IStockService, StockService>();
builder.Services.AddScoped<IStockServiceV2, StockServiceV2>();
builder.Services.AddScoped<IStockServiceV3, StockServiceV3>();

// Step 4A: Commercial services
builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<ILeadRepository, LeadRepository>();
builder.Services.AddScoped<ILeadService, LeadService>();

// Step 4B: Orders & Deliveries
builder.Services.AddScoped<IBonCommandeRepository, BonCommandeRepository>();
builder.Services.AddScoped<IBonLivraisonRepository, BonLivraisonRepository>();
builder.Services.AddScoped<IDocumentPdfService, DocumentPdfService>();
builder.Services.AddScoped<IChampSpecifiqueRepository, ChampSpecifiqueRepository>();
builder.Services.AddScoped<IArticleChampValeurRepository, ArticleChampValeurRepository>();
builder.Services.AddScoped<IChampSpecifiqueService, ChampSpecifiqueService>();
builder.Services.AddScoped<IArticleDynamicFieldService, ArticleDynamicFieldService>();
builder.Services.AddScoped<IArticleEtatDetailRepository, ArticleEtatDetailRepository>();
builder.Services.AddScoped<IArticleEtatDetailService, ArticleEtatDetailService>();
builder.Services.AddScoped<ICataloguePublicService, CataloguePublicService>();

// Email service (confirmation des comptes clients)
builder.Services.AddScoped<IEmailService, EmailService>();

// QuestPDF License
QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

var app = builder.Build();

// Ensure Database is Migrated & Seed Default User
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<StockManagementDbContext>();
        context.Database.Migrate();

        // Seed Default Admin User if none exists
        var adminEmail = "admin@capled.com";
        var adminUser = await context.Users.FirstOrDefaultAsync(u => u.Email == adminEmail);
        
        if (adminUser == null)
        {
            var admin = new User
            {
                FullName = "Administrateur",
                Email = adminEmail,
                Role = StockManager.Core.Domain.Enums.UserRole.ADMIN
            };
            var hasher = new Microsoft.AspNetCore.Identity.PasswordHasher<User>();
            admin.PasswordHash = hasher.HashPassword(admin, "Admin123!");
            context.Users.Add(admin);
            await context.SaveChangesAsync();
        }
        else
        {
            // Ensure password is reset for testing if user exists
            var hasher = new Microsoft.AspNetCore.Identity.PasswordHasher<User>();
            adminUser.PasswordHash = hasher.HashPassword(adminUser, "Admin123!");
            await context.SaveChangesAsync();
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred during DB migration/seeding.");
    }
}

app.UseMiddleware<ExceptionMappingMiddleware>();

// Configure the HTTP request pipeline.
// Always enable Swagger for testing the public URL
app.UseSwagger();
app.UseSwaggerUI();

// app.UseHttpsRedirection(); // Removed to prevent CORS / Network Error issues with self-signed SSL certs on local dev

// Ensure uploads directory exists
var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "equipments");
if (!Directory.Exists(uploadsPath))
{
    Directory.CreateDirectory(uploadsPath);
}

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")),
    RequestPath = ""
});

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }
