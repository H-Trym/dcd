using api.Adapters;
using api.Dtos;
using api.Models;
using api.Services;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;

namespace api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    [RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
    public class DrainageStrategiesController : ControllerBase
    {
        private DrainageStrategyService _drainageStrategyService;
        private readonly ILogger<DrainageStrategiesController> _logger;
        private readonly DrainageStrategyAdapter _drainageStrategyAdapter;

        public DrainageStrategiesController(ILogger<DrainageStrategiesController> logger, DrainageStrategyService drainageStrategyService)
        {
            _logger = logger;
            _drainageStrategyService = drainageStrategyService;
            _drainageStrategyAdapter = new DrainageStrategyAdapter();
        }

        [HttpPost(Name = "CreateDrainageStrategy")]
        public Project CreateDrainageStrategy([FromBody] DrainageStrategyDto drainageStrategyDto)
        {
            var drainageStrategy = _drainageStrategyAdapter.Convert(drainageStrategyDto);
            return _drainageStrategyService.CreateDrainageStrategy(drainageStrategy, drainageStrategyDto.SourceCaseId);
        }

        [HttpDelete("{drainageStrategyId}", Name = "DeleteDrainageStrategy")]
        public Project DeleteDrainageStrategy(Guid drainageStrategyId)
        {
            return _drainageStrategyService.DeleteDrainageStrategy(drainageStrategyId);
        }

        [HttpPatch("{drainageStrategyId}", Name = "UpdateDrainageStrategy")]
        public Project UpdateDrainageStrategy([FromRoute] Guid drainageStrategyId, [FromBody] DrainageStrategyDto drainageStrategyDto)
        {
            var drainageStrategy = _drainageStrategyAdapter.Convert(drainageStrategyDto);
            return _drainageStrategyService.UpdateDrainageStrategy(drainageStrategyId, drainageStrategy);
        }
    }
}