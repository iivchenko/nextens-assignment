using Microsoft.Extensions.Options;
using Nextens.Application;

namespace Nextens.Infrastructure
{
    public sealed class CustomerRepository : ICustomerRepository
    {
        private readonly Settings _settings;

        public CustomerRepository(IOptions<Settings> settings)
        {
            _settings = settings.Value;
        }

        public IAsyncEnumerable<Customer> Read()
        {
            return
               Directory
               .GetFiles(_settings.Path)
               .ToAsyncEnumerable()
               .Select(path => new FileInfo(path).Name)
               .Select(path => Guid.Parse(path.Substring(0, 36)))
               .Distinct()
               .Select(id => new Customer(id));
        }
    }
}
