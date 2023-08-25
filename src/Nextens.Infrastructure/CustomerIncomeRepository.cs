using Microsoft.Extensions.Options;
using Nextens.Application;
using System.Text.Json;

namespace Nextens.Infrastructure
{
    public sealed class CustomerIncomeRepository : ICustomerIncomeRepository
    {
        private readonly Settings _settings;

        public CustomerIncomeRepository(IOptions<Settings> settings)
        {
            _settings = settings.Value;
        }

        public IAsyncEnumerable<CustomerIncome> Read(Guid customerId)
        {
            return
                Directory
                    .GetFiles(_settings.Path, $"{customerId}-*.json")
                    .ToAsyncEnumerable()
                    .SelectAwait(path => JsonSerializer.DeserializeAsync<CustomerIncome>(File.OpenRead(path)));
        }
    }
}
