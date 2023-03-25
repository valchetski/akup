using FluentAssertions;
using Pkup.Console.Report;

namespace Pkup.Console.Tests.Report
{
    public class ReportServiceTests
    {
        private readonly ReportService _reportService;

        public ReportServiceTests()
        {
            _reportService = new ReportService(null, null, null, null, null);
        }

        [Fact]
        public void Report_FromDateIsAfterToDate_ShouldThrowException()
        {
            // arrange
            var config = new PkupConfig(string.Empty, DateTimeOffset.Now.AddDays(1), DateTimeOffset.Now, string.Empty, string.Empty, null, Array.Empty<ProjectConfig>());

            // act
            Action act = () => _reportService.Report(config);

            // assert
            act.Should().ThrowExactly<ReportException>();
        }
    }
}
