using GeneralPurpose.Application.Commands.FunStudio;
using GeneralPurpose.Domain.Entities;
using GeneralPurpose.Domain.Models.Requests.FunStudio;
using GeneralPurpose.Domain.Models.Responses.FunStudio;
using GeneralPurpose.Infrastructure.Extensions;
using GeneralPurpose.Infrastructure.Helpers;
using GeneralPurpose.Infrastructure.Persistence.KeyTypedRepositories;
using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace GeneralPurpose.Api.Controllers;

[Route("fun-studio")]
public class FunStudioController: BaseController
{
    [HttpGet("working-units/status")]
    public async Task<IActionResult> GetWorkingUnitStatus([FromServices] IRepository<WorkingUnit, int> workingUnitRepository, CancellationToken cancellationToken)
    {
        var workingUnitId = HttpContext.GetClientMachineHeader();
        if (string.IsNullOrEmpty(workingUnitId) || workingUnitId.Length > 32)
            return BadRequest("Invalid working unit ID.");

        var workingUnit = await workingUnitRepository.FirstOrDefaultAsync(x => x.Identifier == workingUnitId,
            isNoTracking: true, cancellationToken: cancellationToken);

        if (workingUnit?.Id == null) return BadRequest($"Working unit {workingUnitId} is not found!.");
        
        return Ok(new WorkingUnitStatusResponse
        {
            IsActive = workingUnit.IsActive,
            ExpiryDate = workingUnit.ExpireAt,
            Note = workingUnit.Note
        });
    }

    [HttpPost("working-units/sync-transactions")]
    public async Task<IActionResult> SyncTransactions([FromBody] SyncTransactionsRequest request, [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new SyncTransactionsCommand(HttpContext.GetClientMachineHeader(), request),
            cancellationToken);
        return result.Match(Ok, ForwardGeneralResponse);
    }

    [HttpPost("test/GenerateSignature")]
    public IActionResult GenerateSignature([FromBody] TestGenerateSignatureRequest request)
    {
        return Ok(new TestGenerateSignatureResponse
        {
            Identifier = request.Identifier,
            CurrenTime = request.CurrenTime,
            Sig = SecurityHelper.GenerateSignature(request.Identifier, request.CurrenTime)
        });
    }
    
    [HttpPost("test/ValidateGenerateSignature")]
    public IActionResult ValidateSignature([FromBody] TestGenerateSignatureResponse request)
    {
        return Ok(SecurityHelper.IsValidSignature(request.Sig, request.Identifier, request.CurrenTime));
    }
}