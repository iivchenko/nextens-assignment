namespace Nextens.Application.Indicators
{
    // If the Total Capital is larger than (200 000).
    // Calculation: Total Capital = BankBalanceNational + BankbalanceInternational + StockInvestments.
    // Check this indicator only for a customer’s most recent year
    public sealed class WealthTaxIndicator : IIndicator<WealthTaxReportData>
    {
        public const int Threshold = 200000;

        private readonly ICustomerIncomeRepository _incomeRepository;

        public WealthTaxIndicator(ICustomerIncomeRepository incomeRepository)
        {
            _incomeRepository = incomeRepository;
        }

        public async Task<IndicatorReport<WealthTaxReportData>> Process(Guid customerId)
        {
            var income =
                await _incomeRepository
                    .Read(customerId, _ => true)
                    .OrderByDescending(x => x.Year)
                    .FirstOrDefaultAsync();

            if (income is null)
            {
                return new IndicatorReport<WealthTaxReportData> { CustomerId = customerId };
            }

            var capital = income.BankBalanceNational + income.BankbalanceInternational + income.StockInvestments;

            return capital > Threshold
                ? new IndicatorReport<WealthTaxReportData> { CustomerId = customerId, Data = new[] { new WealthTaxReportData(capital, income.Year) } }
                : new IndicatorReport<WealthTaxReportData> { CustomerId = customerId };
        }
    }
}
