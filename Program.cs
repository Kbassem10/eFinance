using StudentRegistrationPortal.Api.Data;
using StudentRegistrationPortal.Api.Repositories;

var builder = WebApplication.CreateBuilder(args);

// ==========================================
// 1. ADO.NET Connection & Data Access Layer
// ==========================================
builder.Services.AddSingleton<IDbConnectionFactory, MySqlConnectionFactory>();
builder.Services.AddScoped<ISqlDataAccess, SqlDataAccess>();

// ==========================================
// 2. Repository Layer Registration
// ==========================================
builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ICourseRepository, CourseRepository>();

// ==========================================
// 3. Controllers & OpenAPI Configuration
// ==========================================
builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

// ==========================================
// 4. Apply Database Migrations (Like Django)
// ==========================================
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' was not found.");

try
{
    DatabaseMigrator.ApplyMigrations(connectionString, app.Logger);
}
catch (Exception ex)
{
    app.Logger.LogWarning(ex, "Could not apply database migrations on startup (e.g. database offline). Ensure MySQL is running.");
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthorization();
app.MapControllers();

app.Run();
