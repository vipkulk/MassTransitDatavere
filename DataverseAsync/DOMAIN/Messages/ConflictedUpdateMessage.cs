namespace DOMAIN.Messages
{
    public sealed class ConflictedUpdateMessage
    {
        public Guid DataverseId { get; set; }
        public Guid RequestId { get; set; }
        public string LogicalName { get; set; }
        public string Operation { get; set; }
        public DateTime TimeStamp { get; set; }
        public Dictionary<string, object> AttributeCollection { get; set; }
    }
}
