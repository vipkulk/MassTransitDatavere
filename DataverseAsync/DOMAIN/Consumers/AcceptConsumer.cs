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
            switch (context.Message.Operation)
            {
                case Operations.Create:
                    var createData = new Entity(context.Message.LogicalName);
                    foreach (var item in context.Message.AttributeCollection)
                    {
                        PupulateEntity(createData, item);

                    }
                    var Id = await _serviceAsync.CreateAsync(createData).ConfigureAwait(false);
                    await context.Publish(new CompleteMessage
                    {
                        Operation = context.Message.Operation,
                        DataverseId = Id,
                        RequestId = context.Message.Id,
                        LogicalName = context.Message.LogicalName,
                        AttributeCollection = context.Message.AttributeCollection,
                        ClientRequest = context.Message.ClientRequest,                        
                    });
                    break;
                case Operations.Update:
                    var updateData = new Entity(context.Message.LogicalName);
                    if (context.Message.AttributeCollection.TryGetValue("RecordId", out var dataverseId))
                    {
                        updateData.Id = new Guid((string)dataverseId);
                        context.Message.AttributeCollection.Remove("RecordId");
                    }
                    foreach (var item in context.Message.AttributeCollection)
                    {
                        PupulateEntity(updateData, item);
                    }
                    if (string.IsNullOrEmpty(_options.Value?.DateTimeColumnForAvoidingFaultyUpdates))
                    {
                        await _serviceAsync.UpdateAsync(updateData).ConfigureAwait(false);
                        await context.Publish(new CompleteMessage
                        {
                            Operation = context.Message.Operation,
                            DataverseId = updateData.Id,
                            RequestId = context.Message.Id,
                            LogicalName = context.Message.LogicalName,
                            AttributeCollection = context.Message.AttributeCollection,
                            ClientRequest = context.Message.ClientRequest
                        });
                    }
                    else
                    {
                        var column = _options?.Value?.DateTimeColumnForAvoidingFaultyUpdates;
                        var modifiedon = (await _serviceAsync.RetrieveAsync(context.Message.LogicalName, updateData.Id, new ColumnSet(column)).ConfigureAwait(false))
                                          .GetAttributeValue<DateTime>(column);
                        if (DateTime.Compare(modifiedon, context.Message.TimeStamp) < 0)
                        {
                            updateData[column] = context.Message.TimeStamp;
                            await _serviceAsync.UpdateAsync(updateData).ConfigureAwait(false);
                            await context.Publish(new CompleteMessage
                            {
                                Operation = context.Message.Operation,
                                DataverseId = updateData.Id,
                                RequestId = context.Message.Id,
                                LogicalName = context.Message.LogicalName,
                                AttributeCollection = context.Message.AttributeCollection,
                                ClientRequest = context.Message.ClientRequest
                            });
                        }
                        await context.Publish(new ConflictedUpdateMessage
                        {
                            TimeStamp = context.Message.TimeStamp,
                            Operation = context.Message.Operation,
                            DataverseId = updateData.Id,
                            RequestId = context.Message.Id,
                            LogicalName = context.Message.LogicalName,
                            AttributeCollection = context.Message.AttributeCollection,
                            ClientRequest = context.Message.ClientRequest
                        });
                    }
                    break;
                case Operations.Execute:
                    var req = new OrganizationRequest(context.Message.LogicalName);
                    foreach (var item in context.Message.AttributeCollection)
                    {
                        PupulateOrgRequest(req, item);
                    }
                    var response = _serviceAsync.Execute(req);
                    var responseDetails = new Dictionary<string, object>();
                    foreach (var item in response.Results)
                    {
                        responseDetails.Add(item.Key, item.Value);
                    }
                    await context.Publish(new CompleteMessage
                    {
                        Operation = context.Message.Operation,
                        RequestId = context.Message.Id,
                        LogicalName = context.Message.LogicalName,
                        AttributeCollection = context.Message.AttributeCollection,
                        Results = responseDetails,
                        ClientRequest= context.Message.ClientRequest
                    });
                    break;
                default:
                    throw new NotImplementedException($"{context.Message.Operation} not implemented");
            }
        }

        private static void PupulateEntity(Entity createData, KeyValuePair<string, object> item)
        {
            switch (item.Key.Split(Operations.ColumnSplitter)[1])
            {
                case nameof(EntityReference):
                    var dict = (Dictionary<string, object>)item.Value;
                    var keyAttributes = new KeyAttributeCollection();
                    foreach (var keyAttribute in (List<object>)dict["keyAttributes"])
                    {
                        var attributeDict = (Dictionary<string, object>)keyAttribute;
                        var key = (string)attributeDict["key"];
                        var value = attributeDict["value"];
                        keyAttributes.Add(key, value);
                    }
                    if (keyAttributes.Count > 0)
                    {
                        createData[item.Key.Split(Operations.ColumnSplitter)[0]] = new EntityReference((string)dict["logicalName"], keyAttributes);
                    }
                    else
                    {
                        createData[item.Key.Split(Operations.ColumnSplitter)[0]] = new EntityReference((string)dict["logicalName"], new Guid((string)dict["id"]));
                    }
                    break;
                case nameof(OptionSetValue):
                    createData[item.Key.Split(Operations.ColumnSplitter)[0]] = new OptionSetValue(Convert.ToInt32(((Dictionary<string, object>)item.Value)["value"]));
                    break;
                case nameof(Money):
                    createData[item.Key.Split(Operations.ColumnSplitter)[0]] = new Money(Convert.ToDecimal(((Dictionary<string, object>)item.Value)["value"]));
                    break;
                case nameof(String):
                    createData.Attributes.Add(item.Key.Split(Operations.ColumnSplitter)[0], item.Value);
                    break;
                case nameof(Boolean):
                    createData.Attributes.Add(item.Key.Split(Operations.ColumnSplitter)[0], Convert.ToBoolean(item.Value));
                    break;
                case nameof(DateTime):
                    createData.Attributes.Add(item.Key.Split(Operations.ColumnSplitter)[0], Convert.ToDateTime(item.Value));
                    break;
                case nameof(Decimal):
                    createData.Attributes.Add(item.Key.Split(Operations.ColumnSplitter)[0], Convert.ToDecimal(item.Value));
                    break;
                case nameof(Double):
                    createData.Attributes.Add(item.Key.Split(Operations.ColumnSplitter)[0], Convert.ToDouble(item.Value));
                    break;
                case nameof(Int32):
                    createData.Attributes.Add(item.Key.Split(Operations.ColumnSplitter)[0], Convert.ToInt32(item.Value));
                    break;
                case nameof(Int64):
                    createData.Attributes.Add(item.Key.Split(Operations.ColumnSplitter)[0], Convert.ToInt64(item.Value));
                    break;
                default:
                    createData.Attributes.Add(item.Key.Split(Operations.ColumnSplitter)[0], item.Value);
                    break;
            }
        }

        private static void PupulateOrgRequest(OrganizationRequest createData, KeyValuePair<string, object> item)
        {
            switch (item.Key.Split(Operations.ColumnSplitter)[1])
            {
                case nameof(EntityReference):
                    var dict = (Dictionary<string, object>)item.Value;
                    var keyAttributes = new KeyAttributeCollection();
                    foreach (var keyAttribute in (List<object>)dict["keyAttributes"])
                    {
                        var attributeDict = (Dictionary<string, object>)keyAttribute;
                        var key = (string)attributeDict["key"];
                        var value = attributeDict["value"];
                        keyAttributes.Add(key, value);
                    }
                    if (keyAttributes.Count > 0)
                    {
                        createData[item.Key.Split(Operations.ColumnSplitter)[0]] = new EntityReference((string)dict["logicalName"], keyAttributes);
                    }
                    else
                    {
                        createData[item.Key.Split(Operations.ColumnSplitter)[0]] = new EntityReference((string)dict["logicalName"], new Guid((string)dict["id"]));
                    }
                    break;
                case nameof(OptionSetValue):
                    createData[item.Key.Split(Operations.ColumnSplitter)[0]] = new OptionSetValue(Convert.ToInt32(((Dictionary<string, object>)item.Value)["value"]));
                    break;
                case nameof(Money):
                    createData[item.Key.Split(Operations.ColumnSplitter)[0]] = new Money(Convert.ToDecimal(((Dictionary<string, object>)item.Value)["value"]));
                    break;
                case nameof(String):
                    createData.Parameters.Add(item.Key.Split(Operations.ColumnSplitter)[0], item.Value);
                    break;
                case nameof(Boolean):
                    createData.Parameters.Add(item.Key.Split(Operations.ColumnSplitter)[0], Convert.ToBoolean(item.Value));
                    break;
                case nameof(DateTime):
                    createData.Parameters.Add(item.Key.Split(Operations.ColumnSplitter)[0], Convert.ToDateTime(item.Value));
                    break;
                case nameof(Decimal):
                    createData.Parameters.Add(item.Key.Split(Operations.ColumnSplitter)[0], Convert.ToDecimal(item.Value));
                    break;
                case nameof(Double):
                    createData.Parameters.Add(item.Key.Split(Operations.ColumnSplitter)[0], Convert.ToDouble(item.Value));
                    break;
                case nameof(Int32):
                    createData.Parameters.Add(item.Key.Split(Operations.ColumnSplitter)[0], Convert.ToInt32(item.Value));
                    break;
                case nameof(Int64):
                    createData.Parameters.Add(item.Key.Split(Operations.ColumnSplitter)[0], Convert.ToInt64(item.Value));
                    break;
                default:
                    createData.Parameters.Add(item.Key.Split(Operations.ColumnSplitter)[0], item.Value);
                    break;
            }
        }
    }
}
