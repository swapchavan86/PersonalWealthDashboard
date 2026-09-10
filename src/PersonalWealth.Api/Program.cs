using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using PersonalWealth.Api.Infrastructure;
using PersonalWealth.Application.Tenancy;
using PersonalWealth.Application.Wealth;
using PersonalWealth.Infrastructure.Documents;
using PersonalWealth.Infrastructure.Persistence;
using PersonalWealth.Infrastructure.Tenancy;
using PersonalWealth.Infrastructure.Wealth;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Personal Wealth API",
        Version = "v1",
        Description = "Versioned API for the Personal Wealth Platform."
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Production: enter a JWT as Bearer <token>."
    });

    options.AddSecurityDefinition("DevelopmentTenant", new OpenApiSecurityScheme
    {
        Name = "X-Tenant-Id",
        Type = SecuritySchemeType.ApiKey,
        In = ParameterLocation.Header,
        Description = "Development only. Supply the tenant GUID used by the local database."
    });

    options.AddSecurityDefinition("DevelopmentUser", new OpenApiSecurityScheme
    {
        Name = "X-Dev-User",
        Type = SecuritySchemeType.ApiKey,
        In = ParameterLocation.Header,
        Description = "Development only. Supply any non-empty local user identifier."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

if (builder.Environment.IsDevelopment())
{
    builder.Services
        .AddAuthentication("Development")
        .AddScheme<AuthenticationSchemeOptions, DevelopmentAuthenticationHandler>("Development", _ => { });
}
else
{
    builder.Services
        .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.Authority = builder.Configuration["Authentication:Authority"]
                ?? throw new InvalidOperationException("Authentication:Authority is required.");
            options.Audience = builder.Configuration["Authentication:Audience"]
                ?? throw new InvalidOperationException("Authentication:Audience is required.");
        });
}

builder.Services.AddAuthorization();
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddDocumentServices(builder.Configuration);
builder.Services.AddScoped<ITenantContext, HttpTenantContext>();
builder.Services.AddScoped<IWealthDashboardQuery, EfWealthDashboardQuery>();

var app = builder.Build();

app.UseExceptionHandler();
app.UseHttpsRedirection();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Personal Wealth API v1");
        options.RoutePrefix = "swagger";
    });
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

public partial class Program;
