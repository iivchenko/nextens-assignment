namespace Nextens.Application.Indicators
{
    public sealed record WealthTaxIndicatorReportData(decimal TotalCapital, uint Year) : ReportData;
}
