using MediatR;
using MTK.Common.Domain.Abstractions;

namespace MTK.Common.Application.Messaging;

public interface IQueryHandler<TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse>>
    where TQuery : IQuery<TResponse>
{

}
