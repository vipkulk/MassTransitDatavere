using DOMAIN.Messages;
using MassTransit;
using Microsoft.Extensions.Options;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace DOMAIN.Consumers
{
    public sealed class AcceptConsumer : IConsumer<AcceptMessage>
    {
        private readonly IOrganizationServiceAsync2 _serviceAsync;
        private readonly IOptions<ConfigurationOptions> _options;

        public AcceptConsumer(IOrganizationServiceAsync2 serviceAsync ,IOptions<ConfigurationOptions> options)
        {
            _serviceAsync = serviceAsync;
            _options = options;
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
                    var Id = await _serviceAsync.CreateAsync(data).ConfigureAwait(false);
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
                    if (string.IsNullOrEmpty(_options.Value?.DateTimeColumnForAvoidingFaultyUpdates))
                    {
                        await _serviceAsync.UpdateAsync(data).ConfigureAwait(false);
                        await context.Publish(new CompleteMessage
                        {
                            Operation = context.Message.Operation,
                            DataverseId = data.Id,
                            RequestId = context.Message.Id,
                            LogicalName = context.Message.LogicalName,
                            AttributeCollection = context.Message.AttributeCollection
                        });
                    }
                    else
                    {
                        var column = _options?.Value?.DateTimeColumnForAvoidingFaultyUpdates;
                        var modifiedon = (await _serviceAsync.RetrieveAsync(context.Message.LogicalName, data.Id, new ColumnSet(column)).ConfigureAwait(false))
                                          .GetAttributeValue<DateTime>(column);
                        if (DateTime.Compare(modifiedon, context.Message.TimeStamp) < 0)
                        {
                            data[column] = context.Message.TimeStamp;
                            await _serviceAsync.UpdateAsync(data).ConfigureAwait(false);
                            await context.Publish(new CompleteMessage
                            {
                                Operation = context.Message.Operation,
                                DataverseId = data.Id,
                                RequestId = context.Message.Id,
                                LogicalName = context.Message.LogicalName,
                                AttributeCollection = context.Message.AttributeCollection
                            });
                        }
                        await context.Publish(new ConflictedUpdateMessage
                        {
                            TimeStamp = context.Message.TimeStamp,
                            Operation = context.Message.Operation,
                            DataverseId = data.Id,
                            RequestId = context.Message.Id,
                            LogicalName = context.Message.LogicalName,
                            AttributeCollection = context.Message.AttributeCollection
                        });
                    }

                    break;
                default:
                    throw new NotImplementedException($"{context.Message.Operation} not implemented");
            }
        }
    }
}
