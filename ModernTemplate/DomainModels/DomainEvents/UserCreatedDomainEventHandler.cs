using MediatR;

namespace ModernTemplate.DomainModels.DomainEvents;

internal sealed class UserCreatedDomainEventHandler
    : INotificationHandler<UserCreatedDomainEvent>
{
}
