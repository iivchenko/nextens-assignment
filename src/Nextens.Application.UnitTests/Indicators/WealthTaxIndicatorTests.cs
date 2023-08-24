using FluentAssertions;
using Nextens.Application.Indicators;
using NSubstitute;
using System.Linq.Expressions;

namespace Nextens.Application.UnitTests.Indicators
{
    public sealed class WealthTaxIndicatorTests
    {
        private readonly ICustomerIncomeRepository _incomeRepository;

        private readonly WealthTaxIndicator _sut;

        public WealthTaxIndicatorTests()
        {
            _incomeRepository = Substitute.For<ICustomerIncomeRepository>();

            _sut = new WealthTaxIndicator(_incomeRepository);
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
        public async Task Process_ShouldReturnEmptyReport_WhenRecentYearTotalCapitalDoesntExceedThreshold()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var incoms = new List<CustomerIncome>
            {
                new(Guid.NewGuid(), customerId, 2014, 0, 0, 50000, 50000, 100001),
                new(Guid.NewGuid(), customerId, 2016, 0, 0, 50000, 50000, 100000),
                new(Guid.NewGuid(), customerId, 2023, 0, 0, 50000, 50000, 100000),
                new(Guid.NewGuid(), customerId, 2017, 0, 0, 50000, 50000, 100000)
            };

            _incomeRepository
                .Read(Arg.Any<Guid>())
                .Returns(incoms.ToAsyncEnumerable());

            // Act
            var report = await _sut.Process(customerId);

            // Assert
            report.CustomerId.Should().Be(customerId);
            report.Data.Should().BeEmpty();
        }

        [Fact]
        public async Task Process_ShouldReturnReport_WhenRecentYearTotalCapitalExceedsThreshold()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var expectedYear = 2023;
            var incoms = new List<CustomerIncome>
            {
                new(Guid.NewGuid(), customerId, 2014, 0, 0, 50000, 50000, 100001),
                new(Guid.NewGuid(), customerId, 2016, 0, 0, 50000, 50000, 100000),
                new(Guid.NewGuid(), customerId, 2023, 0, 0, 50000, 50000, 100001),
                new(Guid.NewGuid(), customerId, 2017, 0, 0, 50000, 50000, 100000)
            };

            _incomeRepository
                .Read(customerId) 
                .Returns(incoms.ToAsyncEnumerable());

            // Act
            var report = await _sut.Process(customerId);

            // Assert
            report.CustomerId.Should().Be(customerId);
            report.Data.Should().HaveCount(1);
            report.Data.Should().Contain(income => income.Year == expectedYear && income.TotalCapital == 200001);
        }
    }
}
