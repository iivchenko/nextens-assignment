using System.Linq.Expressions;

namespace Nextens.Application
{
    public interface ICustomerRepository
    {
        IAsyncEnumerable<Customer> Read();
    }
}
