using Microsoft.EntityFrameworkCore;
using DSW_3P_PR01.Data;
using DSW_3P_PR01.Repositories;
using DSW_3P_PR01.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();

// Repositorio y Servicio de Misiones (cambiar a MisionSQLRepository cuando se use SQL Server)
builder.Services.AddScoped<IMisionRepository, MisionJsonRepository>();
builder.Services.AddScoped<IMisionService, MisionService>();

// Configuración de SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("SqlServerConnection")
    )
);

// Configuración de JWT
var jwt = builder.Configuration.GetSection("Jwt");
var keyString = jwt["Key"] ?? "GZTSi330RvsuRmBOE+A0pZhkdyCkPMss2tshMMdvUYE=";
var key = Convert.FromBase64String(keyString);

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwt["Issuer"] ?? "DSW_3P_EJ01",
            ValidateAudience = true,
            ValidAudience = jwt["Audience"] ?? "DSW_3P_EJ01_Clients",
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });

// Configuración de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("QuestLogFrontend", policy =>
    {
        policy.WithOrigins(
                "http://localhost:5500",
                "http://127.0.0.1:5500"
            )
            .SetIsOriginAllowed(origin =>
            {
                if (Uri.TryCreate(origin, UriKind.Absolute, out var uri))
                {
                    return uri.Host is "localhost" or "127.0.0.1";
                }
                return false;
            })
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "Missions API v1");
        options.RoutePrefix = "api";
    });
}

// El orden de middlewares es crítico en ASP.NET Core:
// 1. CORS
// 2. Authentication
// 3. Authorization
// 4. MapControllers
app.UseCors("QuestLogFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
