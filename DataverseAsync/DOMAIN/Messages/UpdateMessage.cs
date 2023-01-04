namespace DOMAIN.Messages
{
    public sealed class UpdateMessage
    {
        public Guid Id { get; set; }
        public string LogicalName { get; set; }
        public Dictionary<string, object> AttributeCollection { get; set; }
    }
}
