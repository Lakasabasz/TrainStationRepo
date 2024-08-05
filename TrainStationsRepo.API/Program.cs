using Serilog;
using TrainStationsRepo.API;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(
    options => options.AddPolicy(name: "allowall", policyBuilder => policyBuilder.WithOrigins("*")));

builder.Logging.ClearProviders();
builder.Services.AddSerilog(new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("allowall");

app.UseAuthorization();

app.Logger.Log(LogLevel.Information, "Init world");

var instance = World.Instance;

app.Logger.Log(LogLevel.Information, "Loading complete");

app.MapControllers();

app.Run();