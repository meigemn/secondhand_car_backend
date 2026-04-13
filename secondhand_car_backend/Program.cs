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

var app = builder.Build();

// 3. Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Importante: Si vas a usar Usuarios/Login, el orden debe ser:
// app.UseAuthentication(); 
app.UseAuthorization();

app.MapControllers();

app.Run();