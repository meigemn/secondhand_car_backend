using secondhand_car_backend.Models.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;

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
    ///Creacion de la base de datos y tablas para el login/registro de usuarios con Identity
// 1. Leemos la conexión de appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// 2. Registramos el DbContext para usar PostgreSQL
builder.Services.AddDbContext<MeigemnDbContext>(options =>
    options.UseNpgsql(connectionString));

// 3. Activamos el sistema de Identity para que gestione el login/registro
builder.Services.AddIdentityApiEndpoints<IdentityUser>()
    .AddEntityFrameworkStores<MeigemnDbContext>();
builder.Services.AddAuthorization(); // asegura que Identity use los esquemas que se han definido arriba

/// Fin de la configuracion para el login/registro de usuarios con Identity
///---

///---
/// Configuracion para la autenticacion con JWT (Json Web Tokens)
// 1. Obtener la clave secreta de los User Secrets
var jwtSecret = builder.Configuration["AppSettings:Secret"];
var key = System.Text.Encoding.ASCII.GetBytes(jwtSecret);

// 2. Configurar cómo la API validará a los usuarios (Authentication)
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});
/// Fin de la configuracion para la autenticacion con JWT
///---
var app = builder.Build();
app.MapIdentityApi<IdentityUser>(); //Mapear los endpoint de Identity para que esten disponibles en la API

// 3. Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Importante: Si vas a usar Usuarios/Login, el orden debe ser:
app.UseAuthentication(); 
app.UseAuthorization();

app.MapControllers();

app.Run();