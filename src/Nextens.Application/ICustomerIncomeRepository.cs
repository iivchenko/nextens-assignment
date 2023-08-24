namespace Nextens.Application
{
    public interface ICustomerIncomeRepository
    {
        IAsyncEnumerable<CustomerIncome> Read(Guid customerId);
    }
}
