namespace DOMAIN
{
    public sealed class ConfigurationOptions
    {
        public const string Configuration= nameof(Configuration);
        public string DateTimeColumnForAvoidingFaultyUpdates { get; set; }
        public int RateLimitPerMinute { get; set; }
    }
}
