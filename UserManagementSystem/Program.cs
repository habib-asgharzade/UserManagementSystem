using Hangfire;
using Hangfire.SqlServer;
using Microsoft.EntityFrameworkCore;
using UserManagementSystem.Data;
using UserManagementSystem.Jobs;
using UserManagementSystem.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Hangfire
builder.Services.AddHangfire(config => config
    .UseSqlServerStorage(builder.Configuration.GetConnectionString("HangfireConnection"), new SqlServerStorageOptions { 
        PrepareSchemaIfNecessary = true
    }));
builder.Services.AddHangfireServer();
 

// Register services
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<WelcomeMessageJob>();
builder.Services.AddScoped<UploadDocumentJob>();
builder.Services.AddScoped<DocumentProcessingJob>();
builder.Services.AddScoped<CompletionMessageJob>();
builder.Services.AddScoped<NightlyCleanupJob>();

var app = builder.Build();

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

// Hangfire dashboard
app.UseHangfireDashboard();

// Schedule recurring job
RecurringJob.AddOrUpdate<NightlyCleanupJob>(
    "nightly-cleanup",
    job => job.ExecuteAsync(),
    "0 0 * * *");

app.MapControllers();

// Create directories
Directory.CreateDirectory("Uploads");
Directory.CreateDirectory("PDFs");

app.Run();
