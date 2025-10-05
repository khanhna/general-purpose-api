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
    [HttpGet("Status")]
    public async Task<IActionResult> GetWorkingUnitStatus(
        [FromServices] IRepository<WorkingUnit, int> workingUnitRepository,
        [FromServices] IRepository<ImageVintageProcessConfig, int> imageVintageProcessConfigRepository,
        [FromServices] IRepository<ImageCompositionConfig, int> imageCompositionConfigRepository,
        CancellationToken cancellationToken)
    {
        var workingUnitId = HttpContext.GetClientMachineHeader();
        if (string.IsNullOrEmpty(workingUnitId) || workingUnitId.Length > 32)
            return BadRequest("Invalid working unit ID.");

        var workingUnit = await workingUnitRepository.FirstOrDefaultAsync(x => x.Identifier == workingUnitId,
            x => x.Include(w => w.AppSystem).ThenInclude(a => a.ImageCompositionConfigs)
                .Include(w => w.ImageVintageProcessConfigs), true, cancellationToken);
        if (workingUnit?.Id == null) return BadRequest($"Working unit {workingUnitId} is not found!.");

        var generalVintageConfigs = await imageVintageProcessConfigRepository.ListAsync(
            x => x.WorkingUnitId == null && x.IsActive, isNoTracking: true, cancellationToken: cancellationToken);
        var generalCompositionConfigs = await imageCompositionConfigRepository.ListAsync(
            x => x.AppSystemId == null && x.IsActive, isNoTracking: true, cancellationToken: cancellationToken);
        var generalVintageConfigCodes = generalVintageConfigs?.Select(x => x.Code).ToHashSet() ?? [];
        var generalCompositionConfigFileNames = generalCompositionConfigs?.Select(x => x.FileName).ToHashSet() ?? [];

        for (var i = 0; i < generalVintageConfigs!.Count; i++)
        {
            var specificConfig =
                workingUnit?.ImageVintageProcessConfigs?.FirstOrDefault(x =>
                    x.Code == generalVintageConfigs[i].Code && x.IsActive);
            if(specificConfig?.Id == null) continue;

            generalVintageConfigs[i] = specificConfig;
        }
        
        generalVintageConfigs.AddRange(
            workingUnit?.ImageVintageProcessConfigs?.Where(x =>
                !generalVintageConfigCodes.Contains(x.Code) && x.IsActive) ?? []);

        for (var i = 0; i < generalCompositionConfigs!.Count; i++)
        {
            var specificConfig =
                workingUnit?.AppSystem?.ImageCompositionConfigs?.FirstOrDefault(x =>
                    x.FileName == generalCompositionConfigs[i].FileName && x.IsActive);
            if(specificConfig?.Id == null) continue;

            generalCompositionConfigs[i] = specificConfig;
        }

        generalCompositionConfigs.AddRange(
            workingUnit?.AppSystem?.ImageCompositionConfigs?.Where(x =>
                !generalCompositionConfigFileNames.Contains(x.FileName) && x.IsActive) ?? []);
        
        return Ok(new WorkingUnitStatusResponse
        {
            IsActive = workingUnit!.IsActive,
            ExpiryDate = workingUnit.ExpireAt,
            Note = workingUnit.Note,
            SkinRetouchEnabled = workingUnit.SkinRetouchEnabled,
            VintageProcessEnabled = workingUnit.VintageProcessEnabled,
            ImageVintageConfigs = workingUnit.VintageProcessEnabled && generalVintageConfigs.Count > 0
                ? generalVintageConfigs.Where(x => x.IsActive).Select(x =>
                    new ImageVintageConfig
                    {
                        Code = x.Code.ToString(),
                        Contrast = x.Contrast,
                        Grain = x.Grain,
                        Vignette = x.Vignette,
                        Fade = x.Fade,
                        TintIntensity = x.TintIntensity,
                        Dust = x.Dust,
                        Scratches = x.Scratches,
                        Hairs = x.Hairs,
                        Blur = x.Blur,
                        RedAdjustment = x.RedAdjustment,
                        GreenAdjustment = x.GreenAdjustment,
                        BlueAdjustment = x.BlueAdjustment,
                        Brightness = x.Brightness,
                        LastUpdatedTime = x.LastUpdatedTime
                    }).ToArray()
                : [],
            ImageCompositionConfigs =
                workingUnit.VintageProcessEnabled && generalCompositionConfigs.Count > 0
                    ? generalCompositionConfigs.Where(x => x.IsActive).Select(x =>
                        new ImageCompositionConfigResponse
                        {
                            BlendMode = x.BlendMode,
                            FileName = x.FileName,
                            Feather = x.Feather,
                            Threshold = x.Threshold,
                            Opacity = x.Opacity,
                            InvertThreshold = x.InvertThreshold,
                            LastUpdatedTime = x.LastUpdatedTime
                        }).ToArray()
                    : []
        });
    }

    [HttpPost("SyncTransactions")]
    public async Task<IActionResult> SyncTransactions([FromBody] SyncTransactionsRequest request, [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new SyncTransactionsCommand(HttpContext.GetClientMachineHeader(), request),
            cancellationToken);
        return result.Match(Ok, ForwardGeneralResponse);
    }

    [HttpPost("Test/GenerateSignature")]
    public IActionResult GenerateSignature([FromBody] TestGenerateSignatureRequest request)
    {
        return Ok(new TestGenerateSignatureResponse
        {
            Identifier = request.Identifier,
            CurrenTime = request.CurrenTime,
            Sig = SecurityHelper.GenerateSignature(request.Identifier, request.CurrenTime)
        });
    }
    
    [HttpPost("Test/ValidateGenerateSignature")]
    public IActionResult ValidateSignature([FromBody] TestGenerateSignatureResponse request)
    {
        return Ok(SecurityHelper.IsValidSignature(request.Sig, request.Identifier, request.CurrenTime));
    }
}