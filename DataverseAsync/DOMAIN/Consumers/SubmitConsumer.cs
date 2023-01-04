using DOMAIN.Messages;
using MassTransit;

namespace DOMAIN.Consumers
{
    public sealed class SubmitConsumer : IConsumer<SubmitMessage>
    {

        public async Task Consume(ConsumeContext<SubmitMessage> context)
        {
            await context.RespondAsync(new SubmitResponse
            {
                Id = context.Message.Id,
                isSumbitted =true,
            });

            await context.Publish(new AcceptMessage
            {
                Operation= context.Message.Operation,
                Id = context.Message.Id,
                TimeStamp = DateTime.UtcNow,
                LogicalName = context.Message.LogicalName,
                AttributeCollection = context.Message.AttributeCollection,

            });
        }
    }
}
