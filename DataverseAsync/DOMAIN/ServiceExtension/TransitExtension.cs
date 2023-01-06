using DOMAIN.Classes;
using DOMAIN.Consumers;
using DOMAIN.Interfaces;
using DOMAIN.Messages;
using DOMAIN.StateMachines;
using MassTransit;
using Microsoft.ApplicationInsights.DependencyCollector;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.PowerPlatform.Dataverse.Client;

namespace DOMAIN.ServiceExtension
{
    public static class TransitExtension
    {
        public static IServiceCollection ConfigureTransit(this IServiceCollection services,string dataverseConnectionString,string serviceBusConnection)
        {
            services.AddApplicationInsightsTelemetry();
            services.ConfigureTelemetryModule<DependencyTrackingTelemetryModule>((x, y) =>
            {
                x.IncludeDiagnosticSourceActivities.Add("MassTransit");
            });
            services.AddMassTransit(x =>
            {
                x.SetKebabCaseEndpointNameFormatter();
                x.AddSagaStateMachine<CreateStateMachine, CreateState, CreateSagaDefinition>()
                .MessageSessionRepository();
                x.AddConsumer<AcceptConsumer, AcceptConsumerDefinition>();
                x.UsingAzureServiceBus((context, cfg) =>
                {
                    cfg.Host(serviceBusConnection);
                    cfg.Send<AcceptMessage>(x =>
                    {
                        x.UseSessionIdFormatter(context => context.Message.Id.ToString());
                        
                    });
                    cfg.ConfigureEndpoints(context);
                });
            });
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
