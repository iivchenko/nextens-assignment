namespace Nextens.Application.Indicators
{
    public sealed class IncomeVolatilityIndicator : IIndicator<IncomeVolatilityIndicatorReportData>
    {
        public Task<IIndicatorReport<IncomeVolatilityIndicatorReportData>> Process(Guid customerId)
        {
            throw new NotImplementedException();
        }
    }
}
