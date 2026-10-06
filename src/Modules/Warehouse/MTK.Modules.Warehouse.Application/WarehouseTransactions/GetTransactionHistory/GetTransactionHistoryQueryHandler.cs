using AutoMapper;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Warehouse.Domain.WarehouseTransactions;

namespace MTK.Modules.Warehouse.Application.WarehouseTransactions.GetTransactionHistory;

internal sealed class GetTransactionHistoryQueryHandler
    : IQueryHandler<GetTransactionHistoryQuery, Result<List<TransactionDto>>>
{
    private readonly IWarehouseTransactionRepository _transactionRepository;
    private readonly IMapper _mapper;

    public GetTransactionHistoryQueryHandler(
        IWarehouseTransactionRepository transactionRepository,
        IMapper mapper)
    {
        _transactionRepository = transactionRepository;
        _mapper = mapper;
    }

    public async Task<Result<List<TransactionDto>>> Handle(
        GetTransactionHistoryQuery request,
        CancellationToken cancellationToken)
    {
        var transactions = await _transactionRepository.GetAllAsync(cancellationToken);

        // Filter by nomenclature
        if (request.NomenclatureId.HasValue)
        {
            transactions = transactions
                .Where(t => t.NomenclatureId == request.NomenclatureId.Value)
                .ToList();
        }

        // Filter by transaction type
        if (request.TransactionType.HasValue)
        {
            transactions = transactions
                .Where(t => t.TransactionType == request.TransactionType.Value)
                .ToList();
        }

        // Filter by date range
        if (request.StartDate.HasValue)
        {
            transactions = transactions
                .Where(t => t.TransactionDate >= request.StartDate.Value)
                .ToList();
        }

        if (request.EndDate.HasValue)
        {
            transactions = transactions
                .Where(t => t.TransactionDate <= request.EndDate.Value)
                .ToList();
        }

        // Order by date descending
        transactions = transactions
            .OrderByDescending(t => t.TransactionDate)
            .ToList();

        // Pagination
        var pagedTransactions = transactions
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        var dtos = _mapper.Map<List<TransactionDto>>(pagedTransactions);

        return Result.Success(dtos);
    }
}
