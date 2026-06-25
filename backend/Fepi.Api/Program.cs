using Fepi.Api.Data;
using Fepi.Api.DTOs;
using Fepi.Api.Interfaces;
using Fepi.Api.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Scalar.AspNetCore;
using Fepi.Api.Repositories;

var builder = WebApplication.CreateBuilder(args);

// ───────────────────────────────────────────────────────────────────────────
// Base de datos — PostgreSQL vía Npgsql
// ConnectionStrings:Default en appsettings.json / appsettings.Development.json
// ───────────────────────────────────────────────────────────────────────────
builder.Services.AddDbContext<FepiDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)));


builder.Services.AddScoped<IAvanceService, AvanceService>();
builder.Services.AddScoped<IEstimacionService, EstimacionService>();
builder.Services.AddScoped<IBitacoraService, BitacoraService>();
builder.Services.AddScoped<IConvenioService, ConvenioService>();
builder.Services.AddScoped<IContratoService, ContratoService>();
builder.Services.AddScoped<IEntregaRecepcionService, EntregaRecepcionService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IAlertaService, AlertaService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();

builder.Services.AddScoped<IEstimacionRepository, EstimacionRepository>();
builder.Services.AddScoped<IBitacoraNotaRepository, BitacoraNotaRepository>();
builder.Services.AddScoped<IConvenioRepository, ConvenioRepository>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options => options.AddDefaultPolicy(p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

var app = builder.Build();

// Map InvalidOperationException to 400/404 instead of letting it become 500.
app.Use(async (ctx, next) =>
{
    try
    {
        await next(ctx);
    }
    catch (InvalidOperationException ex)
    {
        ctx.Response.StatusCode = ex.Message.Contains("no encontrado", StringComparison.OrdinalIgnoreCase)
            || ex.Message.Contains("no existe", StringComparison.OrdinalIgnoreCase)
            ? StatusCodes.Status404NotFound
            : StatusCodes.Status400BadRequest;
        ctx.Response.ContentType = "application/json";
        await ctx.Response.WriteAsJsonAsync(new { error = ex.Message });
    }
});

await SeedData.InitializeAsync(app.Services);

app.UseSwagger();

app.UseSwaggerUI();

app.MapScalarApiReference(options =>
{
    options.Title = "API FEPI Documentation";
    options.Theme = ScalarTheme.Purple;
});

app.UseCors();
app.MapControllers();
app.Run();