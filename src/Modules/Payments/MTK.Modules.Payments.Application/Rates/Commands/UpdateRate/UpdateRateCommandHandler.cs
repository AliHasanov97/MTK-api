using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Application.Abstractions.Data;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.Rates.Commands.UpdateRate;

internal sealed class UpdateRateCommandHandler : ICommandHandler<UpdateRateCommand>
{
    private readonly IRateRepository _rateRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateRateCommandHandler(
        IRateRepository rateRepository,
        IUnitOfWork unitOfWork)
    {
        _rateRepository = rateRepository;
        _unitOfWork = unitOfWork;
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

        return Result.Success();
    }
}
