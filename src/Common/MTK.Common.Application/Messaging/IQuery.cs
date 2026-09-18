using MediatR;
using MTK.Common.Domain.Abstractions;

namespace MTK.Common.Application.Messaging;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>
{

}
