using MassTransit;

namespace DOMAIN.StateMachines
{
    public class CreateState : SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }
        public string CurrentState { get; set; }
    }
}