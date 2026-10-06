using AutoMapper;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Warehouse.Domain.WarehouseTransactions;

namespace MTK.Modules.Warehouse.Application.WarehouseTransactions.GetTransactionById;

internal sealed class GetTransactionByIdQueryHandler
    : IQueryHandler<GetTransactionByIdQuery, Result<TransactionResponse>>
{
    private readonly IWarehouseTransactionRepository _transactionRepository;
    private readonly IMapper _mapper;

    public GetTransactionByIdQueryHandler(
        IWarehouseTransactionRepository transactionRepository,
        IMapper mapper)
    {
        _transactionRepository = transactionRepository;
        _mapper = mapper;
    }

    public async Task<Result<TransactionResponse>> Handle(
        GetTransactionByIdQuery request,
        CancellationToken cancellationToken)
    {
        var transaction = await _transactionRepository.GetByIdAsync(request.Id, cancellationToken);

        if (transaction is null)
        {
            return Result.Failure<TransactionResponse>(WarehouseTransactionErrors.NotFound(request.Id));
        }

        var response = _mapper.Map<TransactionResponse>(transaction);

        return Result.Success(response);
    }
}
