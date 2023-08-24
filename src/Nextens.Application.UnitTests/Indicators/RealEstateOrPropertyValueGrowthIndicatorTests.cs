using FluentAssertions;
using Nextens.Application.Indicators;
using NSubstitute;

namespace Nextens.Application.UnitTests.Indicators
{
    public sealed class RealEstateOrPropertyValueGrowthIndicatorTests
    {
        private readonly ICustomerIncomeRepository _incomeRepository;

        private readonly RealEstateOrPropertyValueGrowthIndicator _sut;

        public RealEstateOrPropertyValueGrowthIndicatorTests()
        {
            _incomeRepository = Substitute.For<ICustomerIncomeRepository>();

            _sut = new RealEstateOrPropertyValueGrowthIndicator(_incomeRepository);
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
        public async Task Process_ShouldReturnEmptyReport_WhenLastYearIncomeDontExceedPereviousYears()
        {
            // Arrange
            var customerId = Guid.NewGuid();

            var incoms = new List<CustomerIncome>
            {
                new(Guid.NewGuid(), customerId, 2014, 0, 0, 500, 0, 0),
                new(Guid.NewGuid(), customerId, 2016, 0, 0, 501, 0, 0),
                new(Guid.NewGuid(), customerId, 2023, 0, 0, 500, 0, 0),
                new(Guid.NewGuid(), customerId, 2017, 0, 0, 502, 0, 0)
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
        public async Task Process_ShouldReturnReport_WhenLastYearIncomeExceedPereviousYears()
        {
            // Arrange
            var customerId = Guid.NewGuid();

            var incoms = new List<CustomerIncome>
            {
                new(Guid.NewGuid(), customerId, 2014, 0, 500, 0, 0, 0),
                new(Guid.NewGuid(), customerId, 2016, 0, 501, 0, 0, 0),
                new(Guid.NewGuid(), customerId, 2022, 0, 425, 0, 0, 0), // Expect in the report
                new(Guid.NewGuid(), customerId, 2023, 0, 500, 0, 0, 0), // Recent year
                new(Guid.NewGuid(), customerId, 2017, 0, 502, 0, 0, 0),
                new(Guid.NewGuid(), customerId, 2013, 0, 400, 0, 0, 0), // Expect in the report
                new(Guid.NewGuid(), customerId, 2015, 0, 502, 0, 0, 0),
                new(Guid.NewGuid(), customerId, 2011, 0, 0, 0, 0, 0),   // Expect in the report
                new(Guid.NewGuid(), customerId, 2010, 0, 100, 0, 0, 0)
            };

            _incomeRepository
                .Read(customerId)
                .Returns(incoms.ToAsyncEnumerable());

            // Act
            var report = await _sut.Process(customerId);

            // Assert
            report.CustomerId.Should().Be(customerId);
            report.Data.Should().HaveCount(3);
            report.Data.Should().Contain(x => x.Year == 2022 && x.Increase == 75 && x.PercentageIncrease == 15);
            report.Data.Should().Contain(x => x.Year == 2013 && x.Increase == 100 && x.PercentageIncrease == 20);
            report.Data.Should().Contain(x => x.Year == 2011 && x.Increase == 500 && x.PercentageIncrease == 100);
        }
    }
}
