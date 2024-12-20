using api.AppInfrastructure.Authorization;
using api.AppInfrastructure.ControllerAttributes;
using api.Features.Assets.CaseAssets.Surfs.Dtos;
using api.Features.Assets.CaseAssets.Surfs.Dtos.Create;
using api.Features.Assets.CaseAssets.Surfs.Dtos.Update;
using api.Features.Assets.CaseAssets.Surfs.Services;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;

namespace api.Features.Assets.CaseAssets.Surfs;

[ApiController]
[Route("projects/{projectId}/cases/{caseId}/surfs")]
[RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
[RequiresApplicationRoles(
    ApplicationRole.Admin,
    ApplicationRole.User
)]
[ActionType(ActionType.Edit)]
public class SurfsController(
    ISurfService surfService,
    ISurfTimeSeriesService surfTimeSeriesService) : ControllerBase
{
    [HttpPut("{surfId}")]
    public async Task<SurfDto> UpdateSurf(
        [FromRoute] Guid projectId,
        [FromRoute] Guid caseId,
        [FromRoute] Guid surfId,
        [FromBody] APIUpdateSurfDto dto)
    {
        return await surfService.UpdateSurf(projectId, caseId, surfId, dto);
    }

    [HttpPost("{surfId}/cost-profile-override/")]
    public async Task<SurfCostProfileOverrideDto> CreateSurfCostProfileOverride(
        [FromRoute] Guid projectId,
        [FromRoute] Guid caseId,
        [FromRoute] Guid surfId,
        [FromBody] CreateSurfCostProfileOverrideDto dto)
    {
        return await surfTimeSeriesService.CreateSurfCostProfileOverride(projectId, caseId, surfId, dto);
    }

    [HttpPut("{surfId}/cost-profile-override/{costProfileId}")]
    public async Task<SurfCostProfileOverrideDto> UpdateSurfCostProfileOverride(
        [FromRoute] Guid projectId,
        [FromRoute] Guid caseId,
        [FromRoute] Guid surfId,
        [FromRoute] Guid costProfileId,
        [FromBody] UpdateSurfCostProfileOverrideDto dto)
    {
        return await surfTimeSeriesService.UpdateSurfCostProfileOverride(projectId, caseId, surfId, costProfileId, dto);
    }
}
