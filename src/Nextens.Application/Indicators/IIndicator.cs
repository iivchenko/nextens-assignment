namespace Nextens.Application.Indicators
{
    public interface IIndicator<TReportData>
    {
        Task<IndicatorReport<TReportData>> Process(Guid customerId);
    }
}
