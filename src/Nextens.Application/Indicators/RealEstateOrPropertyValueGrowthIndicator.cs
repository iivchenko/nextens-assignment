namespace Nextens.Application.Indicators
{
    // If the RealEstatePropertyValue of the customer’s most 
    // recent year has increased by at least 15% compared to any of 3 previous years.
    // Display total percentage gain compared to each of the 3 years and the total values.

    public sealed class RealEstateOrPropertyValueGrowthIndicator : IIndicator<RealEstateOrPropertyValueGrowthIndicatorReportData>
    {
        public readonly decimal PercentageIncreaseThreshold = 15;
        public readonly int NumberOfPreviousYearsThreshold = 3;

        private readonly ICustomerIncomeRepository _incomeRepository;

        public RealEstateOrPropertyValueGrowthIndicator(ICustomerIncomeRepository incomeRepository)
        {
            _incomeRepository = incomeRepository;
        }

        public async Task<IndicatorReport<RealEstateOrPropertyValueGrowthIndicatorReportData>> Process(Guid customerId)
        {
            var incomes =
                await _incomeRepository
                    .Read(customerId)
                    .OrderByDescending(x => x.Year)
                    .ToListAsync();

            if (incomes.Count <= 1)
            {
                return new IndicatorReport<RealEstateOrPropertyValueGrowthIndicatorReportData> { CustomerId = customerId };
            }

            var recentImcome = incomes[0];

            incomes.Remove(recentImcome);

            var data = 
                incomes
                    .Where(x => recentImcome.RealEstatePropertyValue > x.RealEstatePropertyValue)
                    .Select(x => new { Income = x, Increase = (recentImcome.RealEstatePropertyValue - x.RealEstatePropertyValue), PercentageIncrease = (recentImcome.RealEstatePropertyValue - x.RealEstatePropertyValue) * 100 / recentImcome.RealEstatePropertyValue })
                    .Where(x => x.PercentageIncrease >= PercentageIncreaseThreshold)
                    .Take(NumberOfPreviousYearsThreshold)
                    .Select(x => new RealEstateOrPropertyValueGrowthIndicatorReportData
                    (
                        x.Income.Year,
                        x.PercentageIncrease,
                        x.Increase
                    ));

            return new IndicatorReport<RealEstateOrPropertyValueGrowthIndicatorReportData>
            {
                CustomerId = customerId,
                Data = data
            };
        }
    }
}
