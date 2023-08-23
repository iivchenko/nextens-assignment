namespace Nextens.Application.Indicators
{
    public sealed class IncomeVolatilityIndicator : IIndicator<IncomeVolatilityIndicatorReportData>
    {
        public Task<IndicatorReport<IncomeVolatilityIndicatorReportData>> Process(Guid customerId)
        {
            throw new NotImplementedException();
        }
    }
}
