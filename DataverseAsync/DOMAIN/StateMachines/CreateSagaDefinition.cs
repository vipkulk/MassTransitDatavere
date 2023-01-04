using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DOMAIN.StateMachines
{
    public sealed class CreateSagaDefinition : SagaDefinition<CreateState>
    {
        protected override void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator, ISagaConfigurator<CreateState> sagaConfigurator)
        {
            if(endpointConfigurator is IServiceBusReceiveEndpointConfigurator sb)
            {
                sb.RequiresSession = true;
            }
        }
    }
}
