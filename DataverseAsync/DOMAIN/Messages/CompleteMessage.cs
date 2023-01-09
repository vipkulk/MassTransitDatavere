namespace DOMAIN.Messages
{
    public sealed class CompleteMessage
    {
        public Guid DataverseId { get; set; }
        public Guid RequestId { get; set; }
        public string LogicalName { get; set; }
        public string Operation { get; set; }
        public Dictionary<string, object> AttributeCollection { get; set; }
        public Dictionary<string, object> Results { get; set; }
        public object? ClientRequest { get; set; }
    }
}
