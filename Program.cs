using System.Reflection;
using Asp.Versioning;
using DbUp;
using FluentValidation;
using FluentValidation.AspNetCore;
using MySqlConnector;
using StudentRegistrationPortal.Api.Converters;
using StudentRegistrationPortal.Api.Repositories;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' was not found.");

// ==========================================
// 1. Direct MySQL ADO.NET Data Source
// ==========================================
builder.Services.AddMySqlDataSource(connectionString);

// ==========================================
// 2. Unit of Work Layer
// ==========================================
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// ==========================================
// 3. API Versioning Configuration
// ==========================================
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = ApiVersionReader.Combine(
        new UrlSegmentApiVersionReader(),
        new HeaderApiVersionReader("X-Api-Version"),
        new QueryStringApiVersionReader("api-version")
    );
}).AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

// ==========================================
// 4. FluentValidation & Controllers
// ==========================================
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new LenientStringJsonConverter());
        options.JsonSerializerOptions.NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString;
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });
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
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "Student Registration Portal API v1");
        options.RoutePrefix = "swagger";
    });
}

app.UseAuthorization();
app.MapControllers();

app.Run();
