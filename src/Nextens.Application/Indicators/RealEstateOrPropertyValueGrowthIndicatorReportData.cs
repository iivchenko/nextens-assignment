namespace Nextens.Application.Indicators
{
    public sealed record RealEstateOrPropertyValueGrowthIndicatorReportData(uint Year, decimal PercentageIncrease, decimal Increase) : ReportData;
}
