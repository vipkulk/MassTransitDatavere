using DOMAIN.Messages;
using Microsoft.Xrm.Sdk;

namespace DOMAIN.Interfaces
{
    public interface ITransitOrganizationService
    {
        public Task<SubmitResponse> Create(Entity entity);
        public Task<SubmitResponse> Update(Entity entity);

    }
}
