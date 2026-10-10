using AutoMapper;
using MTK.Common.Application.Storage;
using MTK.Common.Application.Messaging;

using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Application.Abstractions.Data;
using MTK.Modules.Hr.Application.ApplicationsForChangeOfPosition;
using MTK.Modules.Hr.Application.CompensationOrders;
using MTK.Modules.Hr.Application.Employees;
using MTK.Modules.Hr.Application.EmploymentOrders;
using MTK.Modules.Hr.Application.JobApplications;
using MTK.Modules.Hr.Application.NoticesOfChangeInWorkingConditions;
using MTK.Modules.Hr.Application.OrdersForChangeOfPosition;
using MTK.Modules.Hr.Application.UnexcusedAbsences;
using MTK.Modules.Hr.Application.UnpaidLeaveApplications;
using MTK.Modules.Hr.Application.UnpaidLeaveOrders;
using MTK.Modules.Hr.Application.EducationLeaveApplications;
using MTK.Modules.Hr.Application.EducationLeaveOrders;
using MTK.Modules.Hr.Application.VacationCompensationApplications;
using MTK.Modules.Hr.Application.BonusOrders;
using MTK.Modules.Hr.Application.SalaryDeductions;
using MTK.Modules.Hr.Application.EmploymentStatusChangeApplications;
using MTK.Modules.Hr.Application.EmploymentStatusChangeOrders;
using MTK.Modules.Hr.Application.WorkOnNonWorkdayOrders;
using MTK.Modules.Hr.Application.VacationReturnApplications;
using MTK.Modules.Hr.Application.VacationReturnOrders;
using MTK.Modules.Hr.Domain.FileAttachments;
using MTK.Modules.Hr.Domain.VacationApplications;
using MTK.Modules.Hr.Domain.VacationOrders;
using MTK.Modules.Hr.Domain.VacationCompensationApplications;
using MTK.Modules.Hr.Domain.VacationReturnApplications;
using MTK.Modules.Hr.Domain.VacationReturnOrders;
using MTK.Modules.Hr.Domain.Warnings;

namespace MTK.Modules.Hr.Application.FileAttachments.AddFileAttachment;

public class AddFileAttachmentCommandHandler : ICommandHandler<AddFileAttachmentCommand, AddFileAttachmentResponse>
{
    private readonly IFileAttachmentRepository _fileAttachmentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IFileStorageService _fileStorageService;

    public AddFileAttachmentCommandHandler(
        IMapper mapper,
        IUnitOfWork unitOfWork,
        IFileAttachmentRepository fileAttachmentRepository,
        IFileStorageService fileStorageService)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _fileAttachmentRepository = fileAttachmentRepository;
        _fileStorageService = fileStorageService;
    }

    public async Task<Result<AddFileAttachmentResponse>> Handle(AddFileAttachmentCommand request, CancellationToken cancellationToken)
    {
        if (!request.JobApplicationId.HasValue &&
            !request.EmploymentOrderId.HasValue &&
            !request.EmployeeId.HasValue &&
            !request.ApplicationForChangeOfPositionId.HasValue &&
            !request.OrderForChangeOfPositionId.HasValue &&
            !request.UnexcusedAbsenceId.HasValue &&
            !request.NoticeOfChangeInWorkingConditionsId.HasValue &&
            !request.WarningId.HasValue &&
            !request.CompensationOrderId.HasValue &&
            !request.VacationCompensationApplicationId.HasValue &&
            !request.UnpaidLeaveApplicationId.HasValue &&
            !request.UnpaidLeaveOrderId.HasValue &&
            !request.EducationLeaveApplicationId.HasValue &&
            !request.EducationLeaveOrderId.HasValue &&
            !request.VacationApplicationId.HasValue &&
            !request.VacationOrderId.HasValue &&
            !request.BonusOrderId.HasValue &&
            !request.SalaryDeductionId.HasValue &&
            !request.EmploymentStatusChangeApplicationId.HasValue &&
            !request.EmploymentStatusChangeOrderId.HasValue &&
            !request.WorkOnNonWorkdayOrderId.HasValue &&
            !request.VacationReturnApplicationId.HasValue &&
            !request.VacationReturnOrderId.HasValue)
        {
            return Result.Failure<AddFileAttachmentResponse>(FileAttachmentErrorMessages.InvalidRequest);
        }

        await using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);
        string? uploadedFilePath = null;

        try
        {
            var fileId = Guid.NewGuid();

            using var stream = request.File.OpenReadStream();
            uploadedFilePath = await _fileStorageService.UploadAsync(
                stream,
                FileAttachmentStorageKey.For(fileId),
                request.File.ContentType,
                cancellationToken);

            var extension = Path.GetExtension(request.File.FileName).TrimStart('.').ToLowerInvariant();

            var fileAttachment = FileAttachment.Create(
                id: fileId,
                fileName: request.File.FileName,
                mimeType: extension,
                isPublic: false,
                jobApplicationId: request.JobApplicationId,
                employmentOrderId: request.EmploymentOrderId,
                employeeId: request.EmployeeId,
                applicationForChangeOfPositionId: request.ApplicationForChangeOfPositionId,
                orderForChangeOfPositionId: request.OrderForChangeOfPositionId,
                unexcusedAbsenceId: request.UnexcusedAbsenceId,
                noticeOfChangeInWorkingConditionsId: request.NoticeOfChangeInWorkingConditionsId,
                warningId: request.WarningId,
                compensationOrderId: request.CompensationOrderId,
                vacationCompensationApplicationId: request.VacationCompensationApplicationId,
                unpaidLeaveApplicationId: request.UnpaidLeaveApplicationId,
                unpaidLeaveOrderId: request.UnpaidLeaveOrderId,
                educationLeaveApplicationId: request.EducationLeaveApplicationId,
                educationLeaveOrderId: request.EducationLeaveOrderId,
                vacationApplicationId: request.VacationApplicationId,
                vacationOrderId: request.VacationOrderId,
                bonusOrderId: request.BonusOrderId,
                salaryDeductionId: request.SalaryDeductionId,
                employmentStatusChangeApplicationId: request.EmploymentStatusChangeApplicationId,
                employmentStatusChangeOrderId: request.EmploymentStatusChangeOrderId,
                workOnNonWorkdayOrderId: request.WorkOnNonWorkdayOrderId,
                vacationReturnApplicationId: request.VacationReturnApplicationId,
                vacationReturnOrderId: request.VacationReturnOrderId,
                documentType: request.DocumentType
            );

            await _fileAttachmentRepository.AddAsync(fileAttachment, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            var response = _mapper.Map<AddFileAttachmentResponse>(fileAttachment);
            return Result.Success(response);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(cancellationToken);

            if (!string.IsNullOrEmpty(uploadedFilePath))
            {
                await _fileStorageService.DeleteAsync(uploadedFilePath);
            }

            return Result.Failure<AddFileAttachmentResponse>(FileAttachmentErrorMessages.CreateError);
        }
    }
}
