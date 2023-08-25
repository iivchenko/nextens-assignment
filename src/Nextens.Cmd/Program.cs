using Microsoft.Extensions.DependencyInjection;
using Nextens.Application;
using Nextens.Application.Indicators;
using Nextens.Cmd;
using Nextens.Infrastructure;

var serviceProvider = 
    new ServiceCollection()
        .Configure<Settings>(options => options.Path = "./data")
        .AddSingleton<ICustomerRepository, CustomerRepository>()
        .AddSingleton<ICustomerIncomeRepository, CustomerIncomeRepository>()
        .AddScoped<IIndicator<WealthTaxIndicatorReportData>, WealthTaxIndicator>()
        .AddScoped<IIndicator<RealEstateOrPropertyValueGrowthIndicatorReportData>, RealEstateOrPropertyValueGrowthIndicator>()
        .AddScoped<IIndicator<IncomeVolatilityIndicatorReportData>, IncomeVolatilityIndicator>()
        .AddSingleton<Application>()
        .BuildServiceProvider();

var app = serviceProvider.GetService<Application>();
await app.Run();