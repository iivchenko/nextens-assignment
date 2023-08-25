using Nextens.Application.Indicators;

namespace Nextens.Cmd;

public sealed class CustomerReport
{
    public Guid Id { get; set; }
    public IndicatorReport<WealthTaxIndicatorReportData>? WealthTaxReport { get; set; } = new();
    public IndicatorReport<RealEstateOrPropertyValueGrowthIndicatorReportData>? RealEstateOrPropertyValueGrowthIndicatorReport { get; set; } = new();
    public IndicatorReport<IncomeVolatilityIndicatorReportData>? IncomeVolatilityIndicatorReport { get; set; } = new();
    public Exception? Error { get; set; }
}