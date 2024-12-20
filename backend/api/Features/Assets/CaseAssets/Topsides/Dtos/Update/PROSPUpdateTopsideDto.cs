using api.Features.CaseProfiles.Dtos.TimeSeries.Update;

namespace api.Features.Assets.CaseAssets.Topsides.Dtos.Update;

public class PROSPUpdateTopsideDto : BaseUpdateTopsideDto
{
    public DateTimeOffset? ProspVersion { get; set; }
}

public class UpdateTopsideCostProfileDto : UpdateTimeSeriesCostDto;
