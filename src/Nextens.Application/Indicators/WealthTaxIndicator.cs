namespace Nextens.Application.Indicators
{
    public sealed class WealthTaxIndicator : IIndicator<WealthTaxReportData>
    {
        public Task<IIndicatorReport<WealthTaxReportData>> Process(Guid customerId)
        {
            throw new NotImplementedException();
        }
    }
}
