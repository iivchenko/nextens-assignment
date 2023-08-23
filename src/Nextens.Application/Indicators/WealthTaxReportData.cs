namespace Nextens.Application.Indicators
{
    public sealed record WealthTaxReportData(decimal TotalCapital, uint Year) : ReportData;
}
