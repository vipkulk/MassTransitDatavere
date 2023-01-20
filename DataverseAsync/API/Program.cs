using DOMAIN;
using DOMAIN.ServiceExtension;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.ConfigureTransit(builder.Configuration["Dataverse"], builder.Configuration,BusType.AzureServiceBus, azureServicebusConfiguration: (context, cfg) =>
{
    cfg.Host(builder.Configuration["ServiceBus"]);
    cfg.ConfigureEndpoints(context);
});
builder.Services.AddApplicationInsightsTelemetry();
var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
