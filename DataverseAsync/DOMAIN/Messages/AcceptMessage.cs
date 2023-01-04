namespace DOMAIN.Messages
{
    public sealed class AcceptMessage
    {
        public Guid Id { get; set; }
        public DateTime TimeStamp { get; set; }
        public string LogicalName { get; set; }
        public string Operation { get; set; }
        public Dictionary<string, object> AttributeCollection { get; set; }
    }
}
