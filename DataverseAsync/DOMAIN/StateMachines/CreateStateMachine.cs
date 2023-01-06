using DOMAIN.Messages;
using MassTransit;

namespace DOMAIN.StateMachines
{
    public class CreateStateMachine : MassTransitStateMachine<CreateState>
    {
        public CreateStateMachine()
        {
           
            Event(() => AcceptData, x => x.CorrelateById(m => m.Message.Id));
            Event(() => CompleteData, x => x.CorrelateById(m => m.Message.RequestId));
            Event(() => ConflictData, x => x.CorrelateById(m => m.Message.RequestId));
            InstanceState(x => x.CurrentState);
            Initially(
                When(AcceptData)
                    .TransitionTo(Accepted)
                     .Then(x => Console.WriteLine($"Id:{x.Saga.CorrelationId} , State:{x.Saga.CurrentState}"))
                );
           During(Accepted,
                When(CompleteData)
                .TransitionTo(Completed)
                .Then(x => Console.WriteLine($"Id:{x.Saga.CorrelationId} , State:{x.Saga.CurrentState}")));
            During(Accepted,
                When(ConflictData)
                .TransitionTo(Conflicted)
                .Then(x => Console.WriteLine($"Id:{x.Saga.CorrelationId} , State:{x.Saga.CurrentState}")));

        }

        public State Accepted { get; private set; }
        public State Completed { get; private set; }
        public State Conflicted { get; private set; }


        public Event<AcceptMessage> AcceptData { get; private set; }
        public Event<CompleteMessage> CompleteData { get; private set; }
        public Event<ConflictedUpdateMessage> ConflictData { get; private set; }
    }

}
