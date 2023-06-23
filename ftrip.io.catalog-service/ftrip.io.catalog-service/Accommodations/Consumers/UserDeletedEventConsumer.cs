using ftrip.io.catalog_service.Accommodations.UseCases.DeleteByHostId;
using ftrip.io.user_service.contracts.Users.Events;
using MassTransit;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.catalog_service.Accommodations.Consumers
{
    public class UserDeletedEventConsumer : IConsumer<UserDeletedEvent>
    {
        private readonly IMediator _mediator;

        public UserDeletedEventConsumer(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task Consume(ConsumeContext<UserDeletedEvent> context)
        {
            var deletedUser = context.Message;

            if (deletedUser.UserType == "Host")
                await _mediator.Send(new DeleteByHostIdRequest { HostId = Guid.Parse(deletedUser.UserId) }, CancellationToken.None);
        }

    }
}
