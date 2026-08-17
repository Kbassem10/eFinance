using System.Reflection;
using DbUp;
using MySqlConnector;
using StudentRegistrationPortal.Api.Repositories;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' was not found.");

// ==========================================
// 1. Direct MySQL ADO.NET Data Source (1-Liner)
// ==========================================
builder.Services.AddMySqlDataSource(connectionString);

// ==========================================
// 2. Repository Layer Registration
// ==========================================
builder.Services.AddScoped<IStudentRepository, StudentRepository>();

// ==========================================
// 3. Controllers & OpenAPI Configuration
// ==========================================
builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

// ==========================================
// 4. Auto-Apply Database Migrations on Startup
// ==========================================
try
{
    EnsureDatabase.For.MySqlDatabase(connectionString);
    var upgrader = DeployChanges.To
        .MySqlDatabase(connectionString)
        .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
        .LogToConsole()
        .Build();

    var result = upgrader.PerformUpgrade();
    if (result.Successful)
    {
        app.Logger.LogInformation("Database migrations applied successfully.");
    }
}
catch (Exception ex)
{
    app.Logger.LogWarning(ex, "Could not apply database migrations on startup. Ensure MySQL is running.");
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthorization();
app.MapControllers();

app.Run();
