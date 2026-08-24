using Asp.Versioning;
using FluentValidation.AspNetCore;
using StudentRegistrationPortal.Api.Converters;
using StudentRegistrationPortal.Application;
using StudentRegistrationPortal.Infrastructure;
using StudentRegistrationPortal.Infrastructure.Persistence.Migrations;

var builder = WebApplication.CreateBuilder(args);

// ==========================================
// 1. Clean Architecture Layer Registrations
// ==========================================
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

// ==========================================
// 2. API Versioning Configuration
// ==========================================
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = false;
    options.ReportApiVersions = true;
    options.ApiVersionReader = new HeaderApiVersionReader("X-Api-Version");
}).AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = false;
});

// ==========================================
// 3. Controllers & Serialization Configuration
// ==========================================
builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new LenientStringJsonConverter());
        options.JsonSerializerOptions.NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString;
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });

// ==========================================
// 4. OpenAPI & Swagger UI Configuration
// ==========================================
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
// 5. Auto-Apply Database Migrations on Startup
// ==========================================
var connectionString = app.Configuration.GetConnectionString("DefaultConnection");
if (!string.IsNullOrWhiteSpace(connectionString))
{
    var migrator = app.Services.GetRequiredService<IDatabaseMigrator>();
    migrator.MigrateDatabase(connectionString);
}

// ==========================================
// 6. HTTP Request Pipeline Configuration
// ==========================================
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "Student Registration Portal API v1");
        options.RoutePrefix = "swagger";
    });
}

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

