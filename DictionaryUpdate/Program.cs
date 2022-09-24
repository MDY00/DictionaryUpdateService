using DictionaryService.Services;
using DictionaryUpdate.ApiHosting;
using DictionaryUpdate.Bootstrap;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDicionaryService(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHostedService<Worker>();

builder.Host.UseWindowsService();
builder.Host.UseSerilog((ctx, lc) => lc.ReadFrom.Configuration(ctx.Configuration));

builder.Logging.AddSerilog(new LoggerConfiguration().WriteTo.File(@"\log.txt", rollingInterval: RollingInterval.Day).CreateLogger());

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpLogging();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

DictService.GetConfig();

app.Run();