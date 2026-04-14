using secondhand_car_backend.Models.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using secondhand_car_backend.Services;
using secondhand_car_backend.Models.Pocos;
using secondhand_car_backend.Models.UnitsOfWork;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using secondhand_car_backend.Models.Repositories;

var builder = WebApplication.CreateBuilder(args);

// 1. Configurar Serilog 
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

// 2. Add services to the container.
builder.Services.AddControllers()
    .AddNewtonsoftJson();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

///---
/// Configuración de la base de datos y Identity
///---

// 1. Leemos la conexión de appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// 2. Registramos el DbContext para usar PostgreSQL
builder.Services.AddDbContext<MeigemnDbContext>(options =>
    options.UseNpgsql(connectionString));

// 3. Configuración de Identity (Personalizada para convivir con JWT manual)
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<MeigemnDbContext>()
    .AddDefaultTokenProviders();

///---
/// Registro de Servicios Propios e Inyección de Dependencias
///---

// Mapeo de la sección AppSettings del JSON a la clase POCO
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

// Registro de la Unidad de Trabajo y Servicios de aplicación
builder.Services.AddScoped<MeigemnUnitOfWork>();
builder.Services.AddScoped<IUsersService, UsersService>();

///---
/// Configuración para la autenticación con JWT (Json Web Tokens)
///---

// 1. Obtener la clave secreta de los User Secrets
var jwtSecret = builder.Configuration["AppSettings:Secret"];
if (string.IsNullOrEmpty(jwtSecret))
{
    throw new Exception("JWT Secret no encontrado en la configuración. Revisa tus User Secrets.");
}

var key = Encoding.ASCII.GetBytes(jwtSecret);

// 2. Configurar cómo la API validará a los usuarios (Authentication)
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
        ClockSkew = TimeSpan.Zero // Elimina el tiempo de gracia de 5 min por defecto
    };
});

builder.Services.AddAuthorization();
builder.Services.AddScoped(typeof(MeigemnRepository<>));

///---
/// Construcción de la aplicación (Middleware Pipeline)
///---

var app = builder.Build();

// NOTA: Se ha eliminado app.MapIdentityApi para usar tus propios controladores personalizados

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Importante: El orden es fundamental para que la seguridad funcione
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();