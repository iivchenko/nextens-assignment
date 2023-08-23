using System.Linq.Expressions;

namespace Nextens.Application
{
    public interface ICustomerIncomeRepository
    {
        IAsyncEnumerable<CustomerIncome> Read(Guid customerId, Expression<Func<CustomerIncome, bool>> filter);
    }
}
