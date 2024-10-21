using Akup.Console.Report;
using FluentAssertions;

namespace Akup.Console.Tests.Report
{
    public class ReportServiceTests
    {
        private readonly ReportService _reportService;

        public ReportServiceTests()
        {
            _reportService = new ReportService(null, null, null, null);
        }

        [Fact]
        public void Report_FromDateIsAfterToDate_ShouldThrowException()
        {
            // arrange
            var config = new ReportConfig()
            {
                FromDate = DateTimeOffset.Now.AddDays(1),
                ToDate = DateTimeOffset.Now,
                AuthorName = string.Empty,
                TemplatePath = string.Empty,
                ReportPath = string.Empty,
                Projects = Array.Empty<ProjectConfig>(),
            };

            // act
            Action act = () => _reportService.Report(config);

            // assert
            act.Should().ThrowExactly<ReportException>();
        }
    }
}
