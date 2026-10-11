using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Application.Abstractions.Data;
using MTK.Modules.Payments.Application.Rates.Services;
using MTK.Modules.Payments.Domain.Rates;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.Rates.Commands.CreateRate;

internal sealed class CreateRateCommandHandler : ICommandHandler<CreateRateCommand, Guid>
{
    private readonly IRateRepository _rateRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICachedRateService _cachedRateService;

    public CreateRateCommandHandler(
        IRateRepository rateRepository,
        IUnitOfWork unitOfWork,
        ICachedRateService cachedRateService)
    {
        _rateRepository = rateRepository;
        _unitOfWork = unitOfWork;
        _cachedRateService = cachedRateService;
    }

    public async Task<Result<Guid>> Handle(CreateRateCommand request, CancellationToken cancellationToken)
    {
        var rate = Rate.Create(
            request.RateType,
            request.Amount,
            request.EffectiveFrom,
            request.Description,
            request.GarageType);

        _rateRepository.Add(rate);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Invalidate rate cache after successful creation
        await _cachedRateService.InvalidateCacheAsync(cancellationToken);

        return rate.Id;
    }
}
