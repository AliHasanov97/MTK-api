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
using MTK.Modules.Hr.Application.VacationApplications;
using MTK.Modules.Hr.Application.VacationCompensationApplications;
using MTK.Modules.Hr.Application.VacationOrders;
using MTK.Modules.Hr.Application.BonusOrders;
using MTK.Modules.Hr.Application.SalaryDeductions;
using MTK.Modules.Hr.Application.EmploymentStatusChangeApplications;
using MTK.Modules.Hr.Application.EmploymentStatusChangeOrders;
using MTK.Modules.Hr.Application.WorkOnNonWorkdayOrders;
using MTK.Modules.Hr.Domain.FileAttachments;
using MTK.Modules.Hr.Domain.VacationReturnApplications;
using MTK.Modules.Hr.Domain.VacationReturnOrders;
using MTK.Modules.Hr.Domain.VacationApplications;
using MTK.Modules.Hr.Domain.VacationCompensationApplications;
using MTK.Modules.Hr.Domain.VacationOrders;
using MTK.Modules.Hr.Domain.Warnings;

namespace MTK.Modules.Hr.Application.FileAttachments.AddBulkFileAttachment;

public class AddBulkFileAttachmentCommandHandler : ICommandHandler<AddBulkFileAttachmentCommand, AddBulkFileAttachmentResponse>
{
    private readonly IFileAttachmentRepository _fileAttachmentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileStorageService _fileStorageService;
    private readonly IMapper _mapper;

    public AddBulkFileAttachmentCommandHandler(
        IUnitOfWork unitOfWork,
        IFileAttachmentRepository fileAttachmentRepository,
        IFileStorageService fileStorageService,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _fileAttachmentRepository = fileAttachmentRepository;
        _fileStorageService = fileStorageService;
        _mapper = mapper;
    }

    public async Task<Result<AddBulkFileAttachmentResponse>> Handle(AddBulkFileAttachmentCommand request, CancellationToken cancellationToken)
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
            return Result.Failure<AddBulkFileAttachmentResponse>(FileAttachmentErrorMessages.InvalidRequest);
        }

        if (request.Files == null || request.Files.Count == 0)
        {
            return Result.Failure<AddBulkFileAttachmentResponse>(FileAttachmentErrorMessages.NoFilesProvided);
        }

        await using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);
        var uploadedFilePaths = new List<string>();

        try
        {
            var fileAttachments = new List<FileAttachment>();
            var fileIds = new List<Guid>();

            foreach (var file in request.Files)
            {
                var fileId = Guid.NewGuid();
                fileIds.Add(fileId);

                using var stream = file.OpenReadStream();
                uploadedFilePaths.Add(await _fileStorageService.UploadAsync(
                    stream,
                    FileAttachmentStorageKey.For(fileId),
                    file.ContentType,
                    cancellationToken));
            }

            for (int i = 0; i < request.Files.Count; i++)
            {
                var extension = Path.GetExtension(request.Files[i].FileName).TrimStart('.').ToLowerInvariant();

                var fileAttachment = FileAttachment.Create(
                    id: fileIds[i],
                    fileName: request.Files[i].FileName,
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

                fileAttachments.Add(fileAttachment);
            }

            foreach (var attachment in fileAttachments)
            {
                await _fileAttachmentRepository.AddAsync(attachment, cancellationToken);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            var response = new AddBulkFileAttachmentResponse
            {
                JobApplicationId = request.JobApplicationId,
                EmploymentOrderId = request.EmploymentOrderId,
                EmployeeId = request.EmployeeId,
                ApplicationForChangeOfPositionId = request.ApplicationForChangeOfPositionId,
                OrderForChangeOfPositionId = request.OrderForChangeOfPositionId,
                UnexcusedAbsenceId = request.UnexcusedAbsenceId,
                NoticeOfChangeInWorkingConditionsId = request.NoticeOfChangeInWorkingConditionsId,
                WarningId = request.WarningId,
                CompensationOrderId = request.CompensationOrderId,
                VacationCompensationApplicationId = request.VacationCompensationApplicationId,
                UnpaidLeaveApplicationId = request.UnpaidLeaveApplicationId,
                UnpaidLeaveOrderId = request.UnpaidLeaveOrderId,
                EducationLeaveApplicationId = request.EducationLeaveApplicationId,
                EducationLeaveOrderId = request.EducationLeaveOrderId,
                VacationApplicationId = request.VacationApplicationId,
                VacationOrderId = request.VacationOrderId,
                BonusOrderId = request.BonusOrderId,
                SalaryDeductionId = request.SalaryDeductionId,
                EmploymentStatusChangeApplicationId = request.EmploymentStatusChangeApplicationId,
                EmploymentStatusChangeOrderId = request.EmploymentStatusChangeOrderId,
                WorkOnNonWorkdayOrderId = request.WorkOnNonWorkdayOrderId,
                VacationReturnApplicationId = request.VacationReturnApplicationId,
                VacationReturnOrderId = request.VacationReturnOrderId,
                FileAttachments = _mapper.Map<List<FileAttachmentItem>>(fileAttachments),
                TotalCount = request.Files.Count,
                SuccessCount = fileAttachments.Count
            };

            return Result.Success(response);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(cancellationToken);

            if (uploadedFilePaths.Count > 0)
            {
                foreach (var objectKey in uploadedFilePaths)
                {
                    await _fileStorageService.DeleteAsync(objectKey);
                }
            }

            return Result.Failure<AddBulkFileAttachmentResponse>(FileAttachmentErrorMessages.CreateError);
        }
    }
}
