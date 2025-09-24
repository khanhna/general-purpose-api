using GeneralPurpose.Application.Commands.FunStudio;
using GeneralPurpose.Domain.Entities;
using GeneralPurpose.Domain.Models.Requests.FunStudio;
using GeneralPurpose.Domain.Models.Responses.FunStudio;
using GeneralPurpose.Infrastructure.Extensions;
using GeneralPurpose.Infrastructure.Helpers;
using GeneralPurpose.Infrastructure.Persistence.KeyTypedRepositories;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GeneralPurpose.Api.Controllers;

[Route("WorkingUnits")]
public class WorkingUnitController: BaseController
{
    [HttpGet("status")]
    public async Task<IActionResult> GetWorkingUnitStatus([FromServices] IRepository<WorkingUnit, int> workingUnitRepository, CancellationToken cancellationToken)
    {
        var workingUnitId = HttpContext.GetClientMachineHeader();
        if (string.IsNullOrEmpty(workingUnitId) || workingUnitId.Length > 32)
            return BadRequest("Invalid working unit ID.");

        var workingUnit = await workingUnitRepository.FirstOrDefaultAsync(x => x.Identifier == workingUnitId,
            x => x.Include(w => w.AppSystem).ThenInclude(a => a.ImageCompositionConfigs)
                .Include(w => w.ImageVintageProcessConfig), true, cancellationToken);

        if (workingUnit?.Id == null) return BadRequest($"Working unit {workingUnitId} is not found!.");

        return Ok(new WorkingUnitStatusResponse
        {
            IsActive = workingUnit.IsActive,
            ExpiryDate = workingUnit.ExpireAt,
            Note = workingUnit.Note,
            SkinRetouchEnabled = workingUnit.SkinRetouchEnabled,
            VintageProcessEnabled = workingUnit.VintageProcessEnabled,
            ImageVintageConfig = workingUnit.VintageProcessEnabled && workingUnit.ImageVintageProcessConfig?.Id != null
                ? new ImageVintageConfig
                {
                    Contrast = workingUnit.ImageVintageProcessConfig.Contrast,
                    Grain = workingUnit.ImageVintageProcessConfig.Grain,
                    Vignette = workingUnit.ImageVintageProcessConfig.Vignette,
                    Fade = workingUnit.ImageVintageProcessConfig.Fade,
                    TintIntensity = workingUnit.ImageVintageProcessConfig.TintIntensity,
                    Dust = workingUnit.ImageVintageProcessConfig.Dust,
                    Scratches = workingUnit.ImageVintageProcessConfig.Scratches,
                    Hairs = workingUnit.ImageVintageProcessConfig.Hairs,
                    Blur = workingUnit.ImageVintageProcessConfig.Blur,
                    RedAdjustment = Convert.ToDecimal(workingUnit.ImageVintageProcessConfig.RedAdjustment) / 255,
                    GreenAdjustment = Convert.ToDecimal(workingUnit.ImageVintageProcessConfig.GreenAdjustment) / 255,
                    BlueAdjustment = Convert.ToDecimal(workingUnit.ImageVintageProcessConfig.BlueAdjustment) / 255,
                    Brightness = workingUnit.ImageVintageProcessConfig.Brightness
                }
                : null,
            ImageCompositionConfigs =
                workingUnit.VintageProcessEnabled && workingUnit.AppSystem?.ImageCompositionConfigs?.Count > 0
                    ? workingUnit.AppSystem.ImageCompositionConfigs.Where(x => x.IsActive).Select(x =>
                        new ImageCompositionConfigResponse
                        {
                            BlendMode = x.BlendMode,
                            FileName = x.FileName,
                            Feather = x.Feather,
                            Threshold = x.Threshold,
                            Opacity = x.Opacity,
                            InvertThreshold = x.InvertThreshold
                        }).ToArray()
                    : []
        });
    }

    [HttpPost("sync-transactions")]
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