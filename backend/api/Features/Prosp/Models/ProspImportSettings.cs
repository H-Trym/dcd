namespace api.Features.Prosp.Models;

public class ProspAppConfigModel
{
    public SubStructureAppConfigModel SubStructure = new();
    public SurfAppConfigModel Surf = new();
    public TopSideAppConfigModel TopSide = new();
    public TransportAppConfigModel Transport = new();
    public OnshorePowerSupplyAppConfigModel OnshorePowerSupply = new();
}

public class SurfAppConfigModel
{
    public string costProfileStartYear { get; set; } = null!;
    public string dG3Date { get; set; } = null!;
    public string dG4Date { get; set; } = null!;
    public string lengthProductionLine { get; set; } = null!;
    public string lengthUmbilicalSystem { get; set; } = null!;
    public string productionFlowLineInt { get; set; } = null!;
    public string artificialLiftInt { get; set; } = null!;
    public string riserCount { get; set; } = null!;
    public string templateCount { get; set; } = null!;
    public string producerCount { get; set; } = null!;
    public string waterInjectorCount { get; set; } = null!;
    public string gasInjectorCount { get; set; } = null!;
    public string versionDate { get; set; } = null!;
    public string costYear { get; set; } = null!;
    public string importedCurrency { get; set; } = null!;
    public string cessationCost { get; set; } = null!;
}

public class TopSideAppConfigModel
{
    public string costProfileStartYear { get; set; } = null!;
    public string dG3Date { get; set; } = null!;
    public string dG4Date { get; set; } = null!;
    public string artificialLiftInt { get; set; } = null!;
    public string dryWeight { get; set; } = null!;
    public string fuelConsumption { get; set; } = null!;
    public string flaredGas { get; set; } = null!;
    public string oilCapacity { get; set; } = null!;
    public string gasCapacity { get; set; } = null!;
    public string waterInjectionCapacity { get; set; } = null!;
    public string producerCount { get; set; } = null!;
    public string waterInjectorCount { get; set; } = null!;
    public string gasInjectorCount { get; set; } = null!;
    public string cO2ShareOilProfile { get; set; } = null!;
    public string cO2ShareGasProfile { get; set; } = null!;
    public string cO2ShareWaterInjectionProfile { get; set; } = null!;
    public string cO2OnMaxOilProfile { get; set; } = null!;
    public string cO2OnMaxGasProfile { get; set; } = null!;
    public string cO2OnMaxWaterInjectionProfile { get; set; } = null!;
    public string facilityOpex { get; set; } = null!;
    public string versionDate { get; set; } = null!;
    public string costYear { get; set; } = null!;
    public string importedCurrency { get; set; } = null!;
    public string peakElectricityImported { get; set; } = null!;
}

public class SubStructureAppConfigModel
{
    public string costProfileStartYear { get; set; } = null!;

    public string dG3Date { get; set; } = null!;

    public string dG4Date { get; set; } = null!;

    public string versionDate { get; set; } = null!;
    public string dryWeight { get; set; } = null!;
    public string conceptInt { get; set; } = null!;
    public string costYear { get; set; } = null!;
    public string importedCurrency { get; set; } = null!;
}

public class TransportAppConfigModel
{
    public string costProfileStartYear { get; set; } = null!;
    public string dG3Date { get; set; } = null!;
    public string dG4Date { get; set; } = null!;
    public string versionDate { get; set; } = null!;
    public string costYear { get; set; } = null!;
    public string importedCurrency { get; set; } = null!;
    public string oilExportPipelineLength { get; set; } = null!;
    public string gasExportPipelineLength { get; set; } = null!;
}

public class OnshorePowerSupplyAppConfigModel
{
    public string costProfileStartYear { get; set; } = null!;
    public string dG4Date { get; set; } = null!;

}
