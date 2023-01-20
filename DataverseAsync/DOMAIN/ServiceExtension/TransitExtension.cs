using DOMAIN.Classes;
using DOMAIN.Consumers;
using DOMAIN.Interfaces;
using MassTransit;
using Microsoft.ApplicationInsights.DependencyCollector;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.PowerPlatform.Dataverse.Client;

namespace DOMAIN.ServiceExtension
{
    public static class TransitExtension
    {
        public static IServiceCollection ConfigureTransit(this IServiceCollection services,string dataverseConnectionString,IConfiguration configuration,BusType busType
            ,Action<IBusRegistrationContext,IServiceBusBusFactoryConfigurator> azureServicebusConfiguration =null
            ,Action<IBusRegistrationContext, IRabbitMqBusFactoryConfigurator> rabbitMQConfiguration = null
            )
        {
            services.AddApplicationInsightsTelemetry();
            services.ConfigureTelemetryModule<DependencyTrackingTelemetryModule>((x, y) =>
            {
                x.IncludeDiagnosticSourceActivities.Add("MassTransit");
            });
            services.AddMassTransit(x =>
            {
                x.SetKebabCaseEndpointNameFormatter();
                x.AddConsumer<AcceptConsumer, AcceptConsumerDefinition>();
                if(busType== BusType.AzureServiceBus)
                {
                    x.UsingAzureServiceBus(azureServicebusConfiguration);
                }
                else
                {
                    x.UsingRabbitMq(rabbitMQConfiguration);
                }
                
            });
            services.Configure<ConfigurationOptions>(configuration.GetSection(ConfigurationOptions.Configuration));
            services.AddSingleton(new ServiceClient(dataverseConnectionString));
            services.AddScoped<IOrganizationServiceAsync2>(x =>
            {
                var service = x.GetService<ServiceClient>();
                return service.Clone();
            });
            services.AddScoped<ITransitOrganizationService,TransitOrganizationService>();
            return services;
        }
    }
}
