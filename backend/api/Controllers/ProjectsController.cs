using api.AppInfrastructure.Authorization;
using api.Dtos;
using api.Dtos.Access;
using api.Features.Projects.GetWithAssets;
using api.Services;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;

namespace api.Controllers;

[ApiController]
[Route("projects")]
[RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
public class ProjectsController(
    GetProjectWithAssetsService getProjectWithAssetsService,
    IProjectService projectService,
    IProjectAccessService projectAccessService)
    : ControllerBase
{
    [HttpPost]
    [RequiresApplicationRoles(
        ApplicationRole.Admin,
        ApplicationRole.User
    )]
    [ActionType(ActionType.Edit)]
    public async Task<ProjectWithAssetsDto> CreateProject([FromQuery] Guid contextId)
    {
        var projectId = await projectService.CreateProject(contextId);

        return await getProjectWithAssetsService.GetProjectWithAssets(projectId);
    }

    [RequiresApplicationRoles(
        ApplicationRole.Admin,
        ApplicationRole.User
    )]
    [HttpPut("{projectId}")]
    [ActionType(ActionType.Edit)]
    public async Task<ProjectWithCasesDto> UpdateProject([FromRoute] Guid projectId, [FromBody] UpdateProjectDto projectDto)
    {
        return await projectService.UpdateProject(projectId, projectDto);
    }

    [RequiresApplicationRoles(
        ApplicationRole.Admin,
        ApplicationRole.User
    )]
    [HttpPut("{projectId}/exploration-operational-well-costs/{explorationOperationalWellCostsId}")]
    [ActionType(ActionType.Edit)]
    public async Task<ExplorationOperationalWellCostsDto> UpdateExplorationOperationalWellCosts([FromRoute] Guid projectId, [FromRoute] Guid explorationOperationalWellCostsId, [FromBody] UpdateExplorationOperationalWellCostsDto dto)
    {
        return await projectService.UpdateExplorationOperationalWellCosts(projectId, explorationOperationalWellCostsId, dto);
    }

    [RequiresApplicationRoles(
        ApplicationRole.Admin,
        ApplicationRole.User
    )]
    [HttpPut("{projectId}/development-operational-well-costs/{developmentOperationalWellCostsId}")]
    [ActionType(ActionType.Edit)]
    public async Task<DevelopmentOperationalWellCostsDto> UpdateDevelopmentOperationalWellCosts([FromRoute] Guid projectId, [FromRoute] Guid developmentOperationalWellCostsId, [FromBody] UpdateDevelopmentOperationalWellCostsDto dto)
    {
        return await projectService.UpdateDevelopmentOperationalWellCosts(projectId, developmentOperationalWellCostsId, dto);
    }

    [RequiresApplicationRoles(
        ApplicationRole.Admin,
        ApplicationRole.ReadOnly,
        ApplicationRole.User
    )]
    [HttpGet("{projectId}/access")]
    [ActionType(ActionType.Read)]
    public async Task<AccessRightsDto> GetAccess(Guid projectId)
    {
        return await projectAccessService.GetUserProjectAccess(projectId);
    }
}
