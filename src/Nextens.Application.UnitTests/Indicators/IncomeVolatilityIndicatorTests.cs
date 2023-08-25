using FluentAssertions;
using Nextens.Application.Indicators;
using NSubstitute;

namespace Nextens.Application.UnitTests.Indicators
{
    public sealed class IncomeVolatilityIndicatorTests
    {
        private readonly ICustomerIncomeRepository _incomeRepository;

        private readonly IncomeVolatilityIndicator _sut;

        public IncomeVolatilityIndicatorTests()
        {
            _incomeRepository = Substitute.For<ICustomerIncomeRepository>();

            _sut = new IncomeVolatilityIndicator(_incomeRepository);
        }

        [Fact]
        public async Task Process_ShouldReturnEmptyReport_WhenCustomerHasNoIncome()
        {
            // Arrange
            var customerId = Guid.NewGuid();

            _incomeRepository
                .Read(customerId)
                .Returns(Enumerable.Empty<CustomerIncome>().ToAsyncEnumerable());

            // Act
            var report = await _sut.Process(customerId);

            // Assert
            report.CustomerId.Should().Be(customerId);
            report.Data.Should().BeEmpty();
        }

        [Fact]
        public async Task Process_ShouldReturnEmptyReport_WhenYearsDontExceed50Percent()
        {
            // Arrange
            var customerId = Guid.NewGuid();

            var incoms = new List<CustomerIncome>
            {
                new(Guid.NewGuid(), customerId, 2014, 100, 0, 500, 0, 0),
                new(Guid.NewGuid(), customerId, 2016, 99, 0, 501, 0, 0),
                new(Guid.NewGuid(), customerId, 2023, 98, 0, 500, 0, 0),
                new(Guid.NewGuid(), customerId, 2017, 97, 0, 502, 0, 0)
            };

            _incomeRepository
                .Read(customerId)
                .Returns(incoms.ToAsyncEnumerable());

            // Act
            var report = await _sut.Process(customerId);

            // Assert
            report.CustomerId.Should().Be(customerId);
            report.Data.Should().BeEmpty();
        }

        [Fact]
        public async Task Process_ShouldReturnEmptyReport_WhenYearsExceed50Percent()
        {
            // Arrange
            var customerId = Guid.NewGuid();

            var incoms = new List<CustomerIncome>
            {
                new(Guid.NewGuid(), customerId, 2023, 10000, 0, 0, 0, 0),
                new(Guid.NewGuid(), customerId, 2022, 1000, 0, 0, 0, 0),
                new(Guid.NewGuid(), customerId, 2021, 5000, 0, 0, 0, 0),
                new(Guid.NewGuid(), customerId, 2020, 4000, 0, 0, 0, 0),
                new(Guid.NewGuid(), customerId, 2019, 0, 0, 0, 0, 0)
            };

            _incomeRepository
                .Read(customerId)
                .Returns(incoms.ToAsyncEnumerable());

            // Act
            var report = await _sut.Process(customerId);

            // Assert
            report.CustomerId.Should().Be(customerId);
            report.Data.Should().HaveCount(3);
            report.Data.Should().Contain(x => x.Year1 == 2023 && x.Year2 == 2022 && x.Income == 9000 && x.Percent == 90);
            report.Data.Should().Contain(x => x.Year1 == 2022 && x.Year2 == 2021 && x.Income == -4000 && x.Percent == -80);
            report.Data.Should().Contain(x => x.Year1 == 2020 && x.Year2 == 2019 && x.Income == 4000 && x.Percent == 100);
        }
    }
}
