using MediatR;

namespace ModernTemplate.ApplicationLayer;

public interface IQuery<TReponse> : IRequest<Result<TReponse>>
{
}
