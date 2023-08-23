namespace Nextens.Application.Indicators
{
    public interface IIndicator<TReportData>
    {
        Task<IIndicatorReport<TReportData>> Process(Guid customerId);
    }
}
