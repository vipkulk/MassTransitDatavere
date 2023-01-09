using DOMAIN.Interfaces;
using DOMAIN.Messages;
using MassTransit;
using Microsoft.Xrm.Sdk;

namespace DOMAIN.Classes
{

    public sealed class TransitOrganizationService : ITransitOrganizationService
    {
        private readonly IPublishEndpoint _publishEndpoint;

        public TransitOrganizationService(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;

        }

        public async Task<SubmitResponse> Create(Entity entity, object? inputRequest = null, CancellationToken cancellationToken=default)
        {
            var attributes = new Dictionary<string, object>();
            var Id = Guid.NewGuid();
            foreach (var item in entity.Attributes)
            {
                attributes.Add($"{item.Key}{Operations.ColumnSplitter}{entity[item.Key].GetType().Name}", entity[item.Key]);
            }
            var createMessage = new AcceptMessage()
            {
                Operation = Operations.Create,
                TimeStamp = DateTime.UtcNow,
                LogicalName = entity.LogicalName,
                AttributeCollection = attributes,
                Id = Id,
                ClientRequest = inputRequest
            };
            await _publishEndpoint.Publish(createMessage,cancellationToken);
            return new SubmitResponse()
            {
                Id = Id,
                isSumbitted = true,
            };
        }

        public async Task<SubmitResponse> Execute(OrganizationRequest request, object? inputRequest = null, CancellationToken cancellationToken = default)
        {
            var parameters = new Dictionary<string, object>();
            var Id = Guid.NewGuid();
            foreach (var item in request.Parameters)
            {
                parameters.Add($"{item.Key}{Operations.ColumnSplitter}{request[item.Key].GetType().Name}", request[item.Key]);
            }
            var executeMessage = new AcceptMessage()
            {
                Operation = Operations.Execute,
                TimeStamp = DateTime.UtcNow,
                LogicalName = request.RequestName,
                AttributeCollection = parameters,
                Id = Id,
                ClientRequest = inputRequest
            };
            await _publishEndpoint.Publish(executeMessage, cancellationToken);
            return new SubmitResponse()
            {
                Id = Guid.NewGuid(),
                isSumbitted = true,
            };
        }

        public async Task<SubmitResponse> Update(Entity entity,object? inputRequest = null, CancellationToken cancellationToken = default)
        {
            if (entity.Id == Guid.Empty)
            {
                return new SubmitResponse
                {
                    isSumbitted = false,
                };
            }
            var Id = Guid.NewGuid();
            var attributes = new Dictionary<string, object>();
            foreach (var item in entity.Attributes)
            {
                attributes.Add($"{item.Key}{Operations.ColumnSplitter}{entity[item.Key].GetType().Name}", entity[item.Key]);
            }
            attributes.Add("RecordId", entity.Id);
            var updateMessage = new AcceptMessage()
            {
                Operation = Operations.Update,
                TimeStamp= DateTime.UtcNow,
                LogicalName = entity.LogicalName,
                AttributeCollection = attributes,
                Id = Guid.NewGuid(),
                ClientRequest = inputRequest
            };
            await _publishEndpoint.Publish(updateMessage, cancellationToken);
            return new SubmitResponse()
            {
                Id = Id,
                isSumbitted = true,
            };
        }
    }
}
