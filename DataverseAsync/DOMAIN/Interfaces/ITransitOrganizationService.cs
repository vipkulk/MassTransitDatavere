using DOMAIN.Messages;
using Microsoft.Xrm.Sdk;

namespace DOMAIN.Interfaces
{
    public interface ITransitOrganizationService
    {
        public Task<SubmitResponse> Create(Entity entity,object? clientRequest= null);
        public Task<SubmitResponse> Update(Entity entity, object? inputRequest = null);
        public Task<SubmitResponse> Execute(OrganizationRequest request);

    }
}
