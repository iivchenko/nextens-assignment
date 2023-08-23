using System.Linq.Expressions;

namespace Nextens.Application
{
    public interface ICustomerRepository
    {
        IAsyncEnumerable<Customer> Read(Expression<Func<Customer, bool>> filter);
    }
}
