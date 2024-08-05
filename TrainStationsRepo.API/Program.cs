using System.Net.Mime;
using Microsoft.AspNetCore.Diagnostics;
using Serilog;
using TrainStationsRepo.API;
using TrainStationsRepo.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(
    options => options.AddPolicy(name: "allowall", policyBuilder => policyBuilder.WithOrigins("*")));

builder.Logging.ClearProviders();
builder.Services.AddSerilog(new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger());

builder.Services.AddExceptionHandler<ExceptionFallback>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler("/error");

app.UseHttpsRedirection();

app.UseCors("allowall");

app.UseAuthorization();

app.Logger.Log(LogLevel.Information, "Init world");

var instance = World.Instance;

app.Logger.Log(LogLevel.Information, "Loading complete");

app.MapControllers();

app.Run();