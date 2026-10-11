using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Application.Abstractions.Data;
using MTK.Modules.Payments.Application.Rates.Services;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.Rates.Commands.UpdateRate;

internal sealed class UpdateRateCommandHandler : ICommandHandler<UpdateRateCommand>
{
    private readonly IRateRepository _rateRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICachedRateService _cachedRateService;

    public UpdateRateCommandHandler(
        IRateRepository rateRepository,
        IUnitOfWork unitOfWork,
        ICachedRateService cachedRateService)
    {
        _rateRepository = rateRepository;
        _unitOfWork = unitOfWork;
        _cachedRateService = cachedRateService;
    }

    public async Task<Result> Handle(UpdateRateCommand request, CancellationToken cancellationToken)
    {
        var rate = await _rateRepository.GetByIdAsync(request.RateId, cancellationToken);

        if (rate is null)
        {
            return Result.Failure(new Error("Rate.NotFound", "Tarif tapılmadı"));
        }

        rate.Update(request.Amount, request.Description);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Invalidate rate cache after successful update
        await _cachedRateService.InvalidateCacheAsync(cancellationToken);

        return Result.Success();
    }
}
