using System.ComponentModel.DataAnnotations;

namespace api.Features.Assets.ProjectAssets.DevelopmentOperationalWellCosts.Dtos;

public class DevelopmentOperationalWellCostsDto
{
    [Required] public Guid Id { get; set; }
    [Required] public Guid ProjectId { get; set; }
    [Required] public double RigUpgrading { get; set; }
    [Required] public double RigMobDemob { get; set; }
    [Required] public double AnnualWellInterventionCostPerWell { get; set; }
    [Required] public double PluggingAndAbandonment { get; set; }
}
