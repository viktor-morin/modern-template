using MediatR;

namespace ModernTemplate.Domain.DomainEvents;

internal sealed class UserCreatedDomainEventHandler
    : INotificationHandler<UserCreatedDomainEvent>
{
}
