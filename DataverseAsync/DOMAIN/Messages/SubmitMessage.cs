using Microsoft.Xrm.Sdk;

namespace DOMAIN.Messages
{
    public sealed class SubmitMessage
    {
        public Guid Id { get; set; }
        public string LogicalName { get; set; }
        public string Operation { get; set; } 
        public Dictionary<string,object> AttributeCollection { get; set; }
    }
}
