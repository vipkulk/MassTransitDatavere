using MassTransit;

namespace DOMAIN.Consumers
{
    public sealed class AcceptConsumerDefinition : ConsumerDefinition<AcceptConsumer>
    {
        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<AcceptConsumer> consumerConfigurator)
        {
            endpointConfigurator.UseRateLimit(800, TimeSpan.FromMinutes(1));
            endpointConfigurator.ConcurrentMessageLimit = 30;
        }
    }
}
