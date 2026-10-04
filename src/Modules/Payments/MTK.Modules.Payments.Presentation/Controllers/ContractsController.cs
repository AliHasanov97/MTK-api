using MediatR;
using Microsoft.AspNetCore.Mvc;
using MTK.Common.Application.Authorization;
using MTK.Modules.Payments.Application.Contracts.Commands.AddContractService;
using MTK.Modules.Payments.Application.Contracts.Commands.ChangeContractStatus;
using MTK.Modules.Payments.Application.Contracts.Commands.CreateContract;
using MTK.Modules.Payments.Application.Contracts.Commands.RemoveContractService;
using MTK.Modules.Payments.Application.Contracts.Commands.SetContractServiceStatus;
using MTK.Modules.Payments.Application.Contracts.Commands.UpdateContract;
using MTK.Modules.Payments.Application.Contracts.Commands.UpdateContractService;
using MTK.Modules.Payments.Application.Contracts.Queries.ExportContract;
using MTK.Modules.Payments.Application.Contracts.Queries.GetContractById;
using MTK.Modules.Payments.Application.Contracts.Queries.SearchContracts;

namespace MTK.Modules.Payments.Presentation.Controllers;

public class ContractsController(ISender sender) : BaseController(sender)
{
    [HttpPost("search")]
    public async Task<IActionResult> SearchContracts(
        [FromBody] SearchContractsQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess
            ? Success(result.Value)
            : BadRequest(result.Error);
    }

    [HttpGet("{contractId:guid}")]
    public async Task<IActionResult> GetContractById(
        Guid contractId,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetContractByIdQuery(contractId), cancellationToken);

        return result.IsSuccess
            ? Success(result.Value)
            : BadRequest(result.Error);
    }

    [HttpGet("{contractId:guid}/export")]
    [RequireAnyRole(Roles.Admin, Roles.BuildingManager, Roles.Accountant)]
    public async Task<IActionResult> ExportContract(
        Guid contractId,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new ExportContractQuery(contractId), cancellationToken);

        return result.IsSuccess
            ? File(result.Value.FileContent, result.Value.ContentType, result.Value.FileName)
            : BadRequest(result.Error);
    }

    [HttpPost]
    [RequireAnyRole(Roles.Admin, Roles.BuildingManager)]
    public async Task<IActionResult> CreateContract(
        [FromBody] CreateContractCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? Success(result.Value, "Müqavilə uğurla yaradıldı")
            : BadRequest(result.Error);
    }

    [HttpPut("{contractId:guid}")]
    [RequireAnyRole(Roles.Admin, Roles.BuildingManager)]
    public async Task<IActionResult> UpdateContract(
        Guid contractId,
        [FromBody] UpdateContractCommand command,
        CancellationToken cancellationToken)
    {
        // Route identifikatoru gövdədəkindən üstündür.
        var result = await _sender.Send(command with { ContractId = contractId }, cancellationToken);

        return result.IsSuccess
            ? Success("Müqavilə uğurla yeniləndi")
            : BadRequest(result.Error);
    }

    [HttpPost("{contractId:guid}/services")]
    [RequireAnyRole(Roles.Admin, Roles.BuildingManager)]
    public async Task<IActionResult> AddContractService(
        Guid contractId,
        [FromBody] AddContractServiceCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command with { ContractId = contractId }, cancellationToken);

        return result.IsSuccess
            ? Success(result.Value, "Xidmət uğurla əlavə edildi")
            : BadRequest(result.Error);
    }

    [HttpPut("{contractId:guid}/services/{serviceId:guid}")]
    [RequireAnyRole(Roles.Admin, Roles.BuildingManager)]
    public async Task<IActionResult> UpdateContractService(
        Guid contractId,
        Guid serviceId,
        [FromBody] UpdateContractServiceCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            command with { ContractId = contractId, ServiceId = serviceId },
            cancellationToken);

        return result.IsSuccess
            ? Success("Xidmət uğurla yeniləndi")
            : BadRequest(result.Error);
    }

    [HttpDelete("{contractId:guid}/services/{serviceId:guid}")]
    [RequireAnyRole(Roles.Admin, Roles.BuildingManager)]
    public async Task<IActionResult> RemoveContractService(
        Guid contractId,
        Guid serviceId,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new RemoveContractServiceCommand(contractId, serviceId),
            cancellationToken);

        return result.IsSuccess
            ? Success("Xidmət silindi")
            : BadRequest(result.Error);
    }

    [HttpPut("{contractId:guid}/services/{serviceId:guid}/status")]
    [RequireAnyRole(Roles.Admin, Roles.BuildingManager)]
    public async Task<IActionResult> SetContractServiceStatus(
        Guid contractId,
        Guid serviceId,
        [FromBody] SetContractServiceStatusCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            command with { ContractId = contractId, ServiceId = serviceId },
            cancellationToken);

        return result.IsSuccess
            ? Success(command.IsActive ? "Xidmət aktivləşdirildi" : "Xidmət dayandırıldı")
            : BadRequest(result.Error);
    }

    [HttpPost("{contractId:guid}/activate")]
    [RequireAnyRole(Roles.Admin, Roles.BuildingManager)]
    public async Task<IActionResult> ActivateContract(
        Guid contractId,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new ActivateContractCommand(contractId), cancellationToken);

        return result.IsSuccess
            ? Success("Müqavilə aktivləşdirildi")
            : BadRequest(result.Error);
    }

    [HttpPost("{contractId:guid}/suspend")]
    [RequireAnyRole(Roles.Admin, Roles.BuildingManager)]
    public async Task<IActionResult> SuspendContract(
        Guid contractId,
        [FromBody] SuspendContractCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command with { ContractId = contractId }, cancellationToken);

        return result.IsSuccess
            ? Success("Müqavilə dayandırıldı")
            : BadRequest(result.Error);
    }

    [HttpPost("{contractId:guid}/terminate")]
    [RequireAnyRole(Roles.Admin, Roles.BuildingManager)]
    public async Task<IActionResult> TerminateContract(
        Guid contractId,
        [FromBody] TerminateContractCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command with { ContractId = contractId }, cancellationToken);

        return result.IsSuccess
            ? Success("Müqavilə ləğv edildi")
            : BadRequest(result.Error);
    }

    [HttpDelete("{contractId:guid}")]
    [RequireAnyRole(Roles.Admin, Roles.BuildingManager)]
    public async Task<IActionResult> DeleteContract(
        Guid contractId,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new DeleteContractCommand(contractId), cancellationToken);

        return result.IsSuccess
            ? Success("Müqavilə silindi")
            : BadRequest(result.Error);
    }
}
