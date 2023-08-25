using Microsoft.Extensions.Options;
using Nextens.Application;
using Nextens.Application.Indicators;
using Nextens.Infrastructure;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace Nextens.Cmd;

public sealed class Application
{
    private readonly Settings _settings;
    private readonly ICustomerRepository _customerRepository;
    private readonly IIndicator<WealthTaxIndicatorReportData> _wealthTaxIndicator;
    private readonly IIndicator<RealEstateOrPropertyValueGrowthIndicatorReportData> _realEstateOrPropertyValueGrowthIndicator;
    private readonly IIndicator<IncomeVolatilityIndicatorReportData> _incomeVolatilityIndicator;

    public Application(
        IOptions<Settings> settings,
        ICustomerRepository customerRepository,
        IIndicator<WealthTaxIndicatorReportData> wealthTaxIndicator,
        IIndicator<RealEstateOrPropertyValueGrowthIndicatorReportData> realEstateOrPropertyValueGrowthIndicator,
        IIndicator<IncomeVolatilityIndicatorReportData> incomeVolatilityIndicator)
    {
        _settings = settings.Value;
        _customerRepository = customerRepository;
        _wealthTaxIndicator = wealthTaxIndicator;
        _realEstateOrPropertyValueGrowthIndicator = realEstateOrPropertyValueGrowthIndicator;
        _incomeVolatilityIndicator = incomeVolatilityIndicator;
    }

    public async Task Run()
    {
        AnsiConsole
            .Write(new FigletText("Nextens")
            .LeftJustified()
            .Color(Color.Blue));

        AnsiConsole.MarkupLine("Greetings! My name is [blue]Nextens Bot[/] or [blue]Nextbot[/] for short :robot:");
        AnsiConsole.MarkupLine("Today I will be your guide through the magic world of taxes.");
        var name = AnsiConsole.Ask<string>("[green]How can I call you?:[/] ");
        AnsiConsole.MarkupLineInterpolated($"Nice to meet you [green]{name}[/]!");
        AnsiConsole.MarkupLine($"Lets get back to business!");
        AnsiConsole.MarkupLine("First we need to specify the path to the customers incomes files/blobs.");

        var displayPath = new TextPath(_settings.Path)
            .RootStyle(new Style(foreground: Color.Red))
            .SeparatorStyle(new Style(foreground: Color.Green))
            .StemStyle(new Style(foreground: Color.Blue))
            .LeafStyle(new Style(foreground: Color.Yellow));

        AnsiConsole.Markup("The current path is: "); AnsiConsole.Write(displayPath); AnsiConsole.WriteLine();

        if (AnsiConsole.Confirm("Would you like to change it?", false))
        {
            _settings.Path = AnsiConsole.Ask<string>("What will be the path?: ");

            var displayPath2 = new TextPath(_settings.Path)
                .RootStyle(new Style(foreground: Color.Red))
                .SeparatorStyle(new Style(foreground: Color.Green))
                .StemStyle(new Style(foreground: Color.Blue))
                .LeafStyle(new Style(foreground: Color.Yellow));

            AnsiConsole.Markup("The new path is: "); AnsiConsole.Write(displayPath2); AnsiConsole.WriteLine();
        }

        var indicators = AnsiConsole.Prompt(
            new MultiSelectionPrompt<string>()
                .Title("Select [green]indicators[/] you want to calculate:")
                .AddChoices(new[] {
                    "A Wealth Tax Indicator",
                    "Real estate / property value growth indicator",
                    "Income volatility indicator"
                }));    

        await _customerRepository
            .Read()
            .SelectAwait(async customer =>
            {
                CustomerReport report;
                try
                {
                    report = new CustomerReport
                    {
                        Id = customer.Id,
                        WealthTaxReport =
                        indicators.Contains("A Wealth Tax Indicator")
                            ? await _wealthTaxIndicator.Process(customer.Id)
                            : new IndicatorReport<WealthTaxIndicatorReportData>(),
                        RealEstateOrPropertyValueGrowthIndicatorReport =
                        indicators.Contains("Real estate / property value growth indicator")
                            ? await _realEstateOrPropertyValueGrowthIndicator.Process(customer.Id)
                            : new IndicatorReport<RealEstateOrPropertyValueGrowthIndicatorReportData>(),
                        IncomeVolatilityIndicatorReport =
                        indicators.Contains("Income volatility indicator")
                            ? await _incomeVolatilityIndicator.Process(customer.Id)
                            : new IndicatorReport<IncomeVolatilityIndicatorReportData>()
                    };
                }
                catch (Exception e)
                {
                    // Comment: I expect here issues with data which we as developers should not fix, hack, bypass in the code
                    // the error should be propagated and data must be fixed manually with proper values.
                    report = new CustomerReport
                    {
                        Id = customer.Id,
                        Error = e
                    };
                }

                return report;
            })
            .Where(x =>
                x.Error != null ||
                x.WealthTaxReport.Data.Any() ||
                x.RealEstateOrPropertyValueGrowthIndicatorReport.Data.Any() ||
                x.IncomeVolatilityIndicatorReport.Data.Any())
            .OrderBy(report => report.Error != null)
            .ThenByDescending(report => report.WealthTaxReport.Data.Any() ? report.WealthTaxReport.Data.First().TotalCapital : 0)
            .ThenByDescending(report =>
                report.RealEstateOrPropertyValueGrowthIndicatorReport.Data.Any()
                    ? report.RealEstateOrPropertyValueGrowthIndicatorReport.Data.Max(x => Math.Abs(x.PercentageIncrease))
                    : 0)
            .ForEachAsync(report =>
            {
                AnsiConsole.MarkupLineInterpolated($"Customer: [green]{report.Id}[/]{Environment.NewLine}");

                if (report.Error != null)
                {
                    AnsiConsole.MarkupLine("[red]Error happened during calculations![/]");
                    AnsiConsole.WriteLine(report.Error.ToString());
                }
                else
                {
                    var layout = new Layout("Root")
                        .SplitColumns(
                             new Layout("Left"),
                             new Layout("Center"),
                             new Layout("Right"));                    

                    layout["Left"].Update(PrintWealthTaxReport(report.WealthTaxReport));
                    layout["Center"].Update(PrintRealEstateOrPropertyValueGrowthIndicatorReport(report.RealEstateOrPropertyValueGrowthIndicatorReport));
                    layout["Right"].Update(PrintIncomeVolatilityIndicatorReport(report.IncomeVolatilityIndicatorReport));

                    AnsiConsole.Write(layout);                   
                }

                AnsiConsole.Write(new Rule());
            });

        AnsiConsole.MarkupLine("[green]The report is ready, just [purple]scroll up[/] to the beginning.[/]");
        AnsiConsole.MarkupLineInterpolated($"Thank you for using our services. Till the next time [green]{name}[/]...");

    }

    private static Renderable PrintWealthTaxReport(IndicatorReport<WealthTaxIndicatorReportData> report)
    {
        if (report.Data.Any())
        {
            var table = new Table
            {
                Title = new TableTitle("A Wealth Tax Indicator")
            };

            table.AddColumn("Year");
            table.AddColumn("Total Capital");

            foreach (var item in report.Data)
            {
                table.AddRow(item.Year.ToString(), item.TotalCapital.ToString("N0"));
            }

            return table;
        }
        else
        {
            return new Panel("X");
        }
    }
    
    private static Renderable PrintRealEstateOrPropertyValueGrowthIndicatorReport(IndicatorReport<RealEstateOrPropertyValueGrowthIndicatorReportData> report)
    {
        if (report.Data.Any())
        {
            var table = new Table
            {
                Title = new TableTitle("Real estate / property value growth indicator")
            };

            table.AddColumn("Year");
            table.AddColumn("Value");
            table.AddColumn("%");

            foreach (var item in report.Data)
            {
                var percent = item.PercentageIncrease.ToString("0.00");
                var color = item.PercentageIncrease >= 0 ? "green" : "red";
                table.AddRow(item.Year.ToString(), item.Increase.ToString(), $"[{color}]{percent}[/]");
            }


            return table;
        }
        else
        {
            return new Panel("X");
        }
    }

    private static Renderable PrintIncomeVolatilityIndicatorReport(IndicatorReport<IncomeVolatilityIndicatorReportData> report)
    {
        if (report.Data.Any())
        {
            var table = new Table
            {
                Title = new TableTitle("Income volatility indicator")
            };

            table.AddColumn("Year 1");
            table.AddColumn("Year 2");
            table.AddColumn("Value");
            table.AddColumn("%");

            foreach (var item in report.Data)
            {
                var percent = item.Percent.ToString("0.00");
                var color = item.Percent >= 0 ? "green" : "red";

                table.AddRow(item.Year1.ToString(), item.Year2.ToString(), item.Income.ToString(), $"[{color}]{percent}[/]");
            }


            return table;
        }
        else
        {
            return new Panel("X");
        }
    }
}
