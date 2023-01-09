using DOMAIN;
using DOMAIN.ServiceExtension;

using Microsoft.Extensions.Azure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.ConfigureTransit(builder.Configuration["Dataverse"], builder.Configuration["ServiceBus"]);
builder.Services.Configure<ConfigurationOptions>(builder.Configuration.GetSection(ConfigurationOptions.Configuration));
builder.Services.AddApplicationInsightsTelemetry();
builder.Services.AddAzureClients(x =>
{
    x.AddServiceBusClient(builder.Configuration["ServiceBus"]);
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
