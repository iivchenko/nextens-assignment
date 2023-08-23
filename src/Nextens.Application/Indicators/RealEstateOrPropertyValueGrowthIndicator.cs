namespace Nextens.Application.Indicators
{
    public sealed class RealEstateOrPropertyValueGrowthIndicator : IIndicator<RealEstateOrPropertyValueGrowthIndicatorReportData>
    {
        public Task<IIndicatorReport<RealEstateOrPropertyValueGrowthIndicatorReportData>> Process(Guid customerId)
        {
            throw new NotImplementedException();
        }
    }
}
