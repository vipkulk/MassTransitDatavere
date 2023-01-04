using DOMAIN.Interfaces;
using DOMAIN.Messages;
using MassTransit;
using Microsoft.Xrm.Sdk;

namespace DOMAIN.Classes
{

    public sealed class TransitOrganizationService : ITransitOrganizationService
    {
        private readonly IBus _bus;
        private readonly IRequestClient<SubmitMessage> _requestClient;

        public TransitOrganizationService(IBus bus, IRequestClient<SubmitMessage> requestClient)
        {
            _bus = bus;
            _requestClient = requestClient;
        }

        public async Task<SubmitResponse> Create(Entity entity)
        {
            var attributes = new Dictionary<string, object>();
            foreach (var item in entity.Attributes)
            {
                attributes.Add(item.Key,item.Value);
            }
            var createMessage = new SubmitMessage()
            {
                Operation = Operations.Create,
                LogicalName = entity.LogicalName,
                AttributeCollection = attributes,
                Id = Guid.NewGuid(),
            };
          return (await _requestClient.GetResponse<SubmitResponse>(createMessage).ConfigureAwait(false)).Message;
        }

        public async Task<SubmitResponse> Update(Entity entity)
        {
            if (entity.Id == Guid.Empty)
            {
                return new SubmitResponse
                {
                    isSumbitted = false,
                };
            }
            var attributes = new Dictionary<string, object>();
            foreach (var item in entity.Attributes)
            {
                attributes.Add(item.Key, item.Value);
            }
            attributes.Add("RecordId", entity.Id);
            var updateMessage = new SubmitMessage()
            {
                Operation = Operations.Update,
                LogicalName = entity.LogicalName,
                AttributeCollection = attributes,
                Id = Guid.NewGuid(),
            };
            return (await _requestClient.GetResponse<SubmitResponse>(updateMessage).ConfigureAwait(false)).Message;
        }
    }
}
