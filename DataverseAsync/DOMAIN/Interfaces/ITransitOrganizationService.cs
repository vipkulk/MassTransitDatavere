using DOMAIN.Messages;
using Microsoft.Xrm.Sdk;

namespace DOMAIN.Interfaces
{
    public interface ITransitOrganizationService
    {
        public Task<SubmitResponse> Create(Entity entity,object? clientRequest= null,CancellationToken cancellationToken = default);
        public Task<SubmitResponse> Update(Entity entity, object? inputRequest = null, CancellationToken cancellationToken = default);
        public Task<SubmitResponse> Execute(OrganizationRequest request, object? inputRequest = null, CancellationToken cancellationToken = default);
    }
}
