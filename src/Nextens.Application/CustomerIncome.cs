namespace Nextens.Application
{
    public sealed record CustomerIncome(
       Guid Id,
       Guid ClientId,
       uint Year,
       decimal Income,
       decimal RealEstatePropertyValue,
       decimal BankBalanceNational,
       decimal BankbalanceInternational,
       decimal StockInvestments
    );
}
