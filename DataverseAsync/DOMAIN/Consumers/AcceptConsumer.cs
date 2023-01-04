using DOMAIN.Messages;
using MassTransit;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;

namespace DOMAIN.Consumers
{
    public sealed class AcceptConsumer : IConsumer<AcceptMessage>
    {
        private readonly IOrganizationServiceAsync2 _serviceAsync;

        public AcceptConsumer(IOrganizationServiceAsync2 serviceAsync)
        {
            _serviceAsync = serviceAsync;
        }
        public async Task Consume(ConsumeContext<AcceptMessage> context)       
        {
            var data = new Entity(context.Message.LogicalName);
            if(context.Message.AttributeCollection.TryGetValue("RecordId", out var dataverseId)) 
            {
                data.Id = new Guid((string)dataverseId);
                context.Message.AttributeCollection.Remove("RecordId");
            }
            foreach (var item in context.Message.AttributeCollection)
            {
                data.Attributes.Add(item.Key, item.Value);
            }
            switch (context.Message.Operation)
            {
                case Operations.Create:
                    var Id = await _serviceAsync.CreateAsync(data);
                    await context.Publish(new CompleteMessage
                    {
                        Operation = context.Message.Operation,
                        DataverseId = Id,
                        RequestId = context.Message.Id,
                        LogicalName = context.Message.LogicalName,
                        AttributeCollection = context.Message.AttributeCollection
                    });
                    break;
                case Operations.Update:
                    await _serviceAsync.UpdateAsync(data);
                    await context.Publish(new CompleteMessage
                    {
                        Operation = context.Message.Operation,
                        DataverseId = data.Id,
                        RequestId = context.Message.Id,
                        LogicalName = context.Message.LogicalName,
                        AttributeCollection = context.Message.AttributeCollection
                    });
                    break;
                default:
                    throw new NotImplementedException($"{context.Message.Operation} not implemented");
            }
        }
    }
}
