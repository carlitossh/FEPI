using Fepi.Api.Data;

using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ───────────────────────────────────────────────────────────────────────────
// Base de datos — PostgreSQL vía Npgsql
// ConnectionStrings:Default en appsettings.json / appsettings.Development.json
// ───────────────────────────────────────────────────────────────────────────
builder.Services.AddDbContext<FepiDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


// ───────────────────────────────────────────────────────────────────────────
// Auth — sin autenticación real en esta fase (instrucción explícita).
// El modelo de roles (RolSistema, UsuarioContrato) ya existe en /Models;
// cuando se conecte JWT, agregar aquí AddAuthentication/AddAuthorization
// y [Authorize(Roles = "...")] en los controllers.
// ───────────────────────────────────────────────────────────────────────────

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options => options.AddDefaultPolicy(p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.UseHttpsRedirection();
app.MapControllers();
app.Run();
