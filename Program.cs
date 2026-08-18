using System.Reflection;
using System.Text;
using Asp.Versioning;
using DbUp;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MySqlConnector;
using StudentRegistrationPortal.Api.Converters;
using StudentRegistrationPortal.Api.Repositories;
using StudentRegistrationPortal.Api.Services;

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
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

// ==========================================
// 3. JWT Authentication & Authorization
// ==========================================
var jwtSecret = builder.Configuration["Jwt:Secret"] 
    ?? "SuperSecretKeyForStudentRegistrationPortal2026SecureAuthentication!";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "StudentRegistrationPortal";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "StudentRegistrationPortalClients";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
    };
});

builder.Services.AddAuthorization();

// ==========================================
// 4. API Versioning Configuration
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
// 5. FluentValidation & Controllers
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

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Components ??= new Microsoft.OpenApi.OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, Microsoft.OpenApi.IOpenApiSecurityScheme>();
        var bearerScheme = new Microsoft.OpenApi.OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = Microsoft.OpenApi.SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = Microsoft.OpenApi.ParameterLocation.Header,
            Description = "Enter your JWT Bearer token."
        };

        document.Components.SecuritySchemes["Bearer"] = bearerScheme;

        document.Security ??= new List<Microsoft.OpenApi.OpenApiSecurityRequirement>();
        document.Security.Add(new Microsoft.OpenApi.OpenApiSecurityRequirement
        {
            [new Microsoft.OpenApi.OpenApiSecuritySchemeReference("Bearer", document)] = new List<string>()
        });

        return Task.CompletedTask;
    });

    options.AddSchemaTransformer((schema, context, cancellationToken) =>
    {
        if (context.JsonPropertyInfo != null)
        {
            if (string.Equals(context.JsonPropertyInfo.Name, "dateOfBirth", StringComparison.OrdinalIgnoreCase))
            {
                schema.Example = System.Text.Json.Nodes.JsonValue.Create("2004-05-15");
            }
            else if (string.Equals(context.JsonPropertyInfo.Name, "admissionDate", StringComparison.OrdinalIgnoreCase))
            {
                schema.Example = System.Text.Json.Nodes.JsonValue.Create("2026-09-01");
            }
        }
        return Task.CompletedTask;
    });
});

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

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
