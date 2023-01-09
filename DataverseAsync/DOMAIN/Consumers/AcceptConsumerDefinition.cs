using MassTransit;
using Microsoft.Extensions.Options;

namespace DOMAIN.Consumers
{
    public sealed class AcceptConsumerDefinition : ConsumerDefinition<AcceptConsumer>
    {
        private readonly IOptions<ConfigurationOptions> _options;

        public AcceptConsumerDefinition(IOptions<ConfigurationOptions> options)
        {
            _options = options;
        }
        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<AcceptConsumer> consumerConfigurator)
        {
            if(_options.Value?.RateLimitPerMinuteForCEProcess > 0)
            {
                endpointConfigurator.UseRateLimit(_options.Value.RateLimitPerMinuteForCEProcess, TimeSpan.FromMinutes(1));
            }
            else
            {
                endpointConfigurator.UseRateLimit(800, TimeSpan.FromMinutes(1));
            }

            endpointConfigurator.ConcurrentMessageLimit = 30;
        }
    }
}
