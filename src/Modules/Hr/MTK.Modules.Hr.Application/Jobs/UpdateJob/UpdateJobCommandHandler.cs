using AutoMapper;
using MTK.Common.Application.Messaging;

using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Application.Abstractions.Data;
using MTK.Modules.Hr.Application.Jobs;
using MTK.Modules.Hr.Domain.Jobs;

namespace MTK.Modules.Hr.Application.Jobs.UpdateJob;

internal sealed class UpdateJobCommandHandler : ICommandHandler<UpdateJobCommand, UpdateJobResponse>
{
    private readonly IJobRepository _jobRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateJobCommandHandler(IJobRepository jobRepository, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _jobRepository = jobRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<UpdateJobResponse>> Handle(UpdateJobCommand request, CancellationToken cancellationToken)
    {
        var job = await _jobRepository.GetByIdDefaultAsync(request.JobId, cancellationToken);
        if (job is null)
            return Result.Failure<UpdateJobResponse>(JobErrors.NotFound);

        job.Update(request.Name);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(_mapper.Map<UpdateJobResponse>(job));
    }
}
