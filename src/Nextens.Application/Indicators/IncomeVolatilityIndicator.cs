namespace Nextens.Application.Indicators
{
    // If the Income between two sequential years is significantly (50% difference) higher or lower, record the following.
    // List the details of the difference by displaying both years, Income and % of change.
    public sealed class IncomeVolatilityIndicator : IIndicator<IncomeVolatilityIndicatorReportData>
    {
        private readonly ICustomerIncomeRepository _incomeRepository;

        public IncomeVolatilityIndicator(ICustomerIncomeRepository incomeRepository)
        {
            _incomeRepository = incomeRepository;
        }

        public async Task<IndicatorReport<IncomeVolatilityIndicatorReportData>> Process(Guid customerId)
        {
            var incomes =
                 await _incomeRepository
                     .Read(customerId)
                     .OrderByDescending(x => x.Year)
                     .ToListAsync();

            if (!incomes.Any())
            {
                return new IndicatorReport<IncomeVolatilityIndicatorReportData> { CustomerId = customerId };
            }

            var pairs = new List<(CustomerIncome, CustomerIncome)>();

            var first = incomes.First();

            foreach (var income in incomes.Skip(1))
            {
                pairs.Add((first, income));
                first = income;
            }

            var data = 
                pairs
                    .Select(x => new
                    {
                        FirstYear = x.Item1.Year,
                        SecondYear = x.Item2.Year,
                        Difference = x.Item1.Income - x.Item2.Income,
                        HigherYearIncome = x.Item1.Income > x.Item2.Income ? x.Item1.Income : x.Item2.Income
                    })
                    .Select(x => new
                    {
                        FirstYear = x.FirstYear,
                        SecondYear = x.SecondYear,
                        Difference = x.Difference,
                        HigherYearIncome = x.HigherYearIncome,
                        PercentageIncrease = x.HigherYearIncome != 0 ? x.Difference * 100 / x.HigherYearIncome : 0
                    })
                    .Where(x => Math.Abs(x.PercentageIncrease) >= 50)
                    .Select(x => new IncomeVolatilityIndicatorReportData
                    (
                        x.FirstYear,
                        x.SecondYear,
                        x.Difference,
                        x.PercentageIncrease
                    ))
                    .ToList();

            return new IndicatorReport<IncomeVolatilityIndicatorReportData> { CustomerId = customerId, Data = data };
        }
    }
}
