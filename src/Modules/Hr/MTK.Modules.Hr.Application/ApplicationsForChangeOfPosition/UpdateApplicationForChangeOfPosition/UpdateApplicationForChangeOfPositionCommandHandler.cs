using AutoMapper;
using MTK.Common.Application.Messaging;

using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Application.Abstractions.Data;
using MTK.Modules.Hr.Domain.Applications;
using MTK.Modules.Hr.Domain.ApplicationsForChangeOfPosition;

namespace MTK.Modules.Hr.Application.ApplicationsForChangeOfPosition.UpdateApplicationForChangeOfPosition;

internal sealed class UpdateApplicationForChangeOfPositionCommandHandler
    : ICommandHandler<UpdateApplicationForChangeOfPositionCommand, UpdateApplicationForChangeOfPositionResponse>
{
    private readonly IApplicationForChangeOfPositionRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateApplicationForChangeOfPositionCommandHandler(
        IApplicationForChangeOfPositionRepository repository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<UpdateApplicationForChangeOfPositionResponse>> Handle(
        UpdateApplicationForChangeOfPositionCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var application = await _repository.GetByIdDefaultAsync(request.Id, cancellationToken);
            if (application is null)
                return Result.Failure<UpdateApplicationForChangeOfPositionResponse>(
                    ApplicationForChangeOfPositionErrors.NotFound);

            if (application.Status == ApplicationStatus.ConvertedToOrder)
                return Result.Failure<UpdateApplicationForChangeOfPositionResponse>(
                    ApplicationForChangeOfPositionErrors.AlreadyConverted);

            application.Update(
                request.NewJobId,
                request.SetDate);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(_mapper.Map<UpdateApplicationForChangeOfPositionResponse>(application));
        }
        catch (NullReferenceException)
        {
            return Result.Failure<UpdateApplicationForChangeOfPositionResponse>(ApplicationForChangeOfPositionErrors.NotFound);
        }
        catch (Exception)
        {
            return Result.Failure<UpdateApplicationForChangeOfPositionResponse>(
                ApplicationForChangeOfPositionErrors.UpdateFailed);
        }
    }
}
