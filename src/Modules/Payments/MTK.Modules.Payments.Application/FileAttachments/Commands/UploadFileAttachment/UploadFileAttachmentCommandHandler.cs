using MTK.Common.Application.Messaging;
using MTK.Common.Application.Storage;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Application.Abstractions.Data;
using MTK.Modules.Payments.Domain.FileAttachments;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.FileAttachments.Commands.UploadFileAttachment;

internal sealed class UploadFileAttachmentCommandHandler : ICommandHandler<UploadFileAttachmentCommand, Guid>
{
    private readonly IFileAttachmentRepository _fileAttachmentRepository;
    private readonly IContractRepository _contractRepository;
    private readonly IVendorRepository _vendorRepository;
    private readonly IPaymentRepository _paymentRepository;
    private readonly IOwnerRepository _ownerRepository;
    private readonly IPurchaseRepository _purchaseRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IFileStorageService _fileStorageService;
    private readonly IUnitOfWork _unitOfWork;

    public UploadFileAttachmentCommandHandler(
        IFileAttachmentRepository fileAttachmentRepository,
        IContractRepository contractRepository,
        IVendorRepository vendorRepository,
        IPaymentRepository paymentRepository,
        IOwnerRepository ownerRepository,
        IPurchaseRepository purchaseRepository,
        ITransactionRepository transactionRepository,
        IFileStorageService fileStorageService,
        IUnitOfWork unitOfWork)
    {
        _fileAttachmentRepository = fileAttachmentRepository;
        _contractRepository = contractRepository;
        _vendorRepository = vendorRepository;
        _paymentRepository = paymentRepository;
        _ownerRepository = ownerRepository;
        _purchaseRepository = purchaseRepository;
        _transactionRepository = transactionRepository;
        _fileStorageService = fileStorageService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(UploadFileAttachmentCommand request, CancellationToken cancellationToken)
    {
        if (!request.ContractId.HasValue && !request.VendorId.HasValue && !request.PaymentId.HasValue
            && !request.OwnerId.HasValue && !request.PurchaseId.HasValue && !request.TransactionId.HasValue)
        {
            return Result.Failure<Guid>(new Error(
                "FileAttachment.NoTarget",
                "Ən azı bir əlaqəli obyekt (müqavilə, tədarükçü, ödəniş, sahib və ya tranzaksiya) göstərilməlidir"));
        }

        if (request.File.Length == 0)
        {
            return Result.Failure<Guid>(new Error("FileAttachment.EmptyFile", "Fayl boşdur"));
        }

        if (request.ContractId.HasValue
            && await _contractRepository.GetByIdDefaultAsync(request.ContractId.Value, cancellationToken) is null)
        {
            return Result.Failure<Guid>(new Error("FileAttachment.ContractNotFound", "Müqavilə tapılmadı"));
        }

        if (request.VendorId.HasValue
            && await _vendorRepository.GetByIdDefaultAsync(request.VendorId.Value, cancellationToken) is null)
        {
            return Result.Failure<Guid>(new Error("FileAttachment.VendorNotFound", "Tədarükçü tapılmadı"));
        }

        if (request.PaymentId.HasValue
            && await _paymentRepository.GetByIdDefaultAsync(request.PaymentId.Value, cancellationToken) is null)
        {
            return Result.Failure<Guid>(new Error("FileAttachment.PaymentNotFound", "Ödəniş tapılmadı"));
        }

        if (request.OwnerId.HasValue
            && await _ownerRepository.GetByIdDefaultAsync(request.OwnerId.Value, cancellationToken) is null)
        {
            return Result.Failure<Guid>(new Error("FileAttachment.OwnerNotFound", "Sahib tapılmadı"));
        }

        if (request.PurchaseId.HasValue
            && await _purchaseRepository.GetByIdDefaultAsync(request.PurchaseId.Value, cancellationToken) is null)
        {
            return Result.Failure<Guid>(new Error("FileAttachment.PurchaseNotFound", "Sat?nalma tap?lmad?"));
        }

        if (request.TransactionId.HasValue
            && await _transactionRepository.GetByIdDefaultAsync(request.TransactionId.Value, cancellationToken) is null)
        {
            return Result.Failure<Guid>(new Error("FileAttachment.TransactionNotFound", "Tranzaksiya tapılmadı"));
        }

        var objectKey = $"payments/{Guid.NewGuid()}-{request.File.FileName}";

        await using (var stream = request.File.OpenReadStream())
        {
            await _fileStorageService.UploadAsync(stream, objectKey, request.File.ContentType, cancellationToken);
        }

        var attachment = FileAttachment.Create(
            request.File.FileName,
            objectKey,
            request.File.ContentType,
            request.File.Length,
            request.ContractId,
            request.VendorId,
            request.PaymentId,
            request.OwnerId,
            request.PurchaseId,
            request.TransactionId,
            request.UploadedByUserId);

        _fileAttachmentRepository.Add(attachment);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(attachment.Id);
    }
}
