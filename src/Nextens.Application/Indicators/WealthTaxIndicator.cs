namespace Nextens.Application.Indicators
{
    // If the Total Capital is larger than (200 000).
    // Calculation: Total Capital = BankBalanceNational + BankbalanceInternational + StockInvestments.
    // Check this indicator only for a customer’s most recent year
    public sealed class WealthTaxIndicator : IIndicator<WealthTaxIndicatorReportData>
    {
        public readonly int TotalCapitalThreshold = 200000;

        private readonly ICustomerIncomeRepository _incomeRepository;

        public WealthTaxIndicator(ICustomerIncomeRepository incomeRepository)
        {
            _incomeRepository = incomeRepository;
        }

        public async Task<IndicatorReport<WealthTaxIndicatorReportData>> Process(Guid customerId)
        {
            var income =
                await _incomeRepository
                    .Read(customerId)
                    .OrderByDescending(x => x.Year)
                    .FirstOrDefaultAsync();

            if (income is null)
            {
                return new IndicatorReport<WealthTaxIndicatorReportData> { CustomerId = customerId };
            }

            var capital = income.BankBalanceNational + income.BankbalanceInternational + income.StockInvestments;

            return capital > TotalCapitalThreshold
                ? new IndicatorReport<WealthTaxIndicatorReportData> { CustomerId = customerId, Data = new[] { new WealthTaxIndicatorReportData(capital, income.Year) } }
                : new IndicatorReport<WealthTaxIndicatorReportData> { CustomerId = customerId };
        }
    }
}
