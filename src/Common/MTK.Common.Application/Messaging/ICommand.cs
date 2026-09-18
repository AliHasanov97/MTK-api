using MediatR;
using MTK.Common.Domain.Abstractions;

namespace MTK.Common.Application.Messaging;

public interface ICommand : IRequest<Result>, IBaseCommand
{

}

public interface ICommand<TResponse> : IRequest<Result<TResponse>>, IBaseCommand
{

}
