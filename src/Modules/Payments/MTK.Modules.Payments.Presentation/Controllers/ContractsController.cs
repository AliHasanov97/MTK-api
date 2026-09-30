using MediatR;
using Microsoft.AspNetCore.Mvc;
using MTK.Modules.Payments.Application.Contracts.Commands.AddContractGoodsItem;
using MTK.Modules.Payments.Application.Contracts.Commands.AddContractService;
using MTK.Modules.Payments.Application.Contracts.Commands.ChangeContractStatus;
using MTK.Modules.Payments.Application.Contracts.Commands.CreateContract;
using MTK.Modules.Payments.Application.Contracts.Commands.RemoveContractGoodsItem;
using MTK.Modules.Payments.Application.Contracts.Commands.RemoveContractService;
using MTK.Modules.Payments.Application.Contracts.Commands.SetContractGoodsItemStatus;
using MTK.Modules.Payments.Application.Contracts.Commands.SetContractServiceStatus;
using MTK.Modules.Payments.Application.Contracts.Commands.UpdateContract;
using MTK.Modules.Payments.Application.Contracts.Commands.UpdateContractGoodsItem;
using MTK.Modules.Payments.Application.Contracts.Commands.UpdateContractService;
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

    [HttpPost]
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

    [HttpPost("{contractId:guid}/goods")]
    public async Task<IActionResult> AddContractGoodsItem(
        Guid contractId,
        [FromBody] AddContractGoodsItemCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command with { ContractId = contractId }, cancellationToken);

        return result.IsSuccess
            ? Success(result.Value, "Mal sətri uğurla əlavə edildi")
            : BadRequest(result.Error);
    }

    [HttpPut("{contractId:guid}/goods/{goodsItemId:guid}")]
    public async Task<IActionResult> UpdateContractGoodsItem(
        Guid contractId,
        Guid goodsItemId,
        [FromBody] UpdateContractGoodsItemCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            command with { ContractId = contractId, GoodsItemId = goodsItemId },
            cancellationToken);

        return result.IsSuccess
            ? Success("Mal sətri uğurla yeniləndi")
            : BadRequest(result.Error);
    }

    [HttpDelete("{contractId:guid}/goods/{goodsItemId:guid}")]
    public async Task<IActionResult> RemoveContractGoodsItem(
        Guid contractId,
        Guid goodsItemId,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new RemoveContractGoodsItemCommand(contractId, goodsItemId),
            cancellationToken);

        return result.IsSuccess
            ? Success("Mal sətri silindi")
            : BadRequest(result.Error);
    }

    [HttpPut("{contractId:guid}/goods/{goodsItemId:guid}/status")]
    public async Task<IActionResult> SetContractGoodsItemStatus(
        Guid contractId,
        Guid goodsItemId,
        [FromBody] SetContractGoodsItemStatusCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            command with { ContractId = contractId, GoodsItemId = goodsItemId },
            cancellationToken);

        return result.IsSuccess
            ? Success(command.IsActive ? "Mal sətri aktivləşdirildi" : "Mal sətri dayandırıldı")
            : BadRequest(result.Error);
    }

    [HttpPost("{contractId:guid}/activate")]
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
