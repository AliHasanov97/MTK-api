using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Application.Abstractions.Data;
using MTK.Modules.Payments.Application.CompanyBalances.Services;
using MTK.Modules.Payments.Domain.Repositories;
using MTK.Modules.Payments.Domain.Transactions;

namespace MTK.Modules.Payments.Application.Transactions.Commands.CreateManualTransaction;

internal sealed class CreateManualTransactionCommandHandler : ICommandHandler<CreateManualTransactionCommand, Guid>
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly ICompanyBalanceService _companyBalanceService;
    private readonly IUnitOfWork _unitOfWork;

    public CreateManualTransactionCommandHandler(
        ITransactionRepository transactionRepository,
        ICompanyBalanceService companyBalanceService,
        IUnitOfWork unitOfWork)
    {
        _transactionRepository = transactionRepository;
        _companyBalanceService = companyBalanceService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateManualTransactionCommand request, CancellationToken cancellationToken)
    {
        var transaction = Transaction.Create(
            request.Direction,
            request.Category,
            request.Amount,
            request.Description,
            DateTimeOffset.UtcNow);

        _transactionRepository.Add(transaction);
        // Must be persisted before RecalculateAsync: it sums via a DB query, which
        // doesn't see an Added-but-unsaved row — see PaymentCompletedDomainEventHandler.
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _companyBalanceService.RecalculateAsync(cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(transaction.Id);
    }
}
