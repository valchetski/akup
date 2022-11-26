using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Pkup.Console.Report;
using System;

namespace Pkup.Console.Tests
{
    public class ReportServiceTests
    {
        private readonly GitConfig _gitConfig;
        private readonly ReportConfig _reportConfig;

        private readonly IReportService _sut;

        public ReportServiceTests()
        {
            var services = HostConfigurator
                .CreateHostBuilder()
                .ConfigureAppConfiguration(x => x.AddJsonFile("appsettings.tests.json"))
            .Build()
            .Services;

            _gitConfig = services.GetRequiredService<IOptions<GitConfig>>().Value;
            _reportConfig = services.GetRequiredService<IOptions<ReportConfig>>().Value;

            _sut = services.GetRequiredService<IReportService>();
        }

        [Fact]
        public void Report_Should_GenerateReport()
        {
            // arrange
            // act
            Action act = () => _sut.Report(
                _gitConfig.RepositoriesSources!,
                _gitConfig.AuthorName!,
                _gitConfig.FromDate,
                _gitConfig.ToDate,
                _reportConfig.ProjectName!,
                _reportConfig.TemplatePath!,
                _reportConfig.ReportPath!,
                _reportConfig.Tokens!);

            // assert
            act.Should().NotThrow();
        }
    }
}