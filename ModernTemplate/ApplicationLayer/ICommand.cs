using MediatR;

namespace ModernTemplate.ApplicationLayer;

public interface ICommand : IRequest<Result>
{
}