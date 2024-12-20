using api.Features.CaseProfiles.Dtos.TimeSeries;
using api.Features.CaseProfiles.Dtos.TimeSeries.Update;
using api.Models;

namespace api.Features.Assets.CaseAssets.DrainageStrategies.Dtos;

public class UpdateDrainageStrategyWithProfilesDto
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public double NGLYield { get; set; }
    public int ProducerCount { get; set; }
    public int GasInjectorCount { get; set; }
    public int WaterInjectorCount { get; set; }
    public ArtificialLift ArtificialLift { get; set; }
    public GasSolution GasSolution { get; set; }
    public UpdateProductionProfileOilDto? ProductionProfileOil { get; set; }
    public UpdateAdditionalProductionProfileOilDto? AdditionalProductionProfileOil { get; set; }
    public UpdateProductionProfileGasDto? ProductionProfileGas { get; set; }
    public UpdateAdditionalProductionProfileGasDto? AdditionalProductionProfileGas { get; set; }
    public UpdateProductionProfileWaterDto? ProductionProfileWater { get; set; }
    public UpdateProductionProfileWaterInjectionDto? ProductionProfileWaterInjection { get; set; }
    public UpdateFuelFlaringAndLossesOverrideDto? FuelFlaringAndLossesOverride { get; set; }
    public UpdateNetSalesGasOverrideDto? NetSalesGasOverride { get; set; }
    public UpdateImportedElectricityOverrideDto? ImportedelectricityOverride { get; set; }

    public UpdateCo2EmissionsOverrideDto? Co2EmissionsOverride { get; set; }
    public UpdateDeferredOilProductionDto? DeferredOilProduction { get; set; }
    public UpdateDeferredGasProductionDto? DeferredGasProduction { get; set; }
}

public class UpdateProductionProfileOilDto : UpdateTimeSeriesVolumeDto;

public class UpdateAdditionalProductionProfileOilDto : UpdateTimeSeriesVolumeDto;

public class UpdateProductionProfileGasDto : UpdateTimeSeriesVolumeDto;

public class UpdateAdditionalProductionProfileGasDto : UpdateTimeSeriesVolumeDto;

public class UpdateProductionProfileWaterDto : UpdateTimeSeriesVolumeDto;

public class UpdateProductionProfileWaterInjectionDto : UpdateTimeSeriesVolumeDto;

public class UpdateFuelFlaringAndLossesOverrideDto : UpdateTimeSeriesVolumeDto, ITimeSeriesOverrideDto
{
    public bool Override { get; set; }
}
public class UpdateNetSalesGasOverrideDto : UpdateTimeSeriesVolumeDto, ITimeSeriesOverrideDto
{
    public bool Override { get; set; }
}

public class UpdateCo2EmissionsOverrideDto : UpdateTimeSeriesMassDto, ITimeSeriesOverrideDto
{
    public bool Override { get; set; }
}

public class UpdateImportedElectricityOverrideDto : UpdateTimeSeriesEnergyDto, ITimeSeriesOverrideDto
{
    public bool Override { get; set; }
}

public class UpdateDeferredOilProductionDto : UpdateTimeSeriesVolumeDto;

public class UpdateDeferredGasProductionDto : UpdateTimeSeriesVolumeDto;
