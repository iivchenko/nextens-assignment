namespace Nextens.Application.Indicators
{
    public sealed record IncomeVolatilityIndicatorReportData(uint Year1, uint Year2, decimal Income, decimal Percent) : ReportData;
}
