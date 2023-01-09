namespace DOMAIN
{
    public sealed class ConfigurationOptions
    {
        public const string Configuration= nameof(Configuration);
        public string DateTimeColumnForAvoidingFaultyUpdates { get; set; }
        public int RateLimitPerMinuteForCEProcess { get; set; }
        public int RateLimitPerMinuteForProcessingStage { get; set; }
    }

    public enum BusType
    {
        AzureServiceBus,
        RabbitMQ
    }
}
