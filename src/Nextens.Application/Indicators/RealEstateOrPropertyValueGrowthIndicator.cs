namespace Nextens.Application.Indicators
{
    public sealed class RealEstateOrPropertyValueGrowthIndicator : IIndicator<RealEstateOrPropertyValueGrowthIndicatorReportData>
    {
        public Task<IndicatorReport<RealEstateOrPropertyValueGrowthIndicatorReportData>> Process(Guid customerId)
        {
            throw new NotImplementedException();
        }
    }
}
