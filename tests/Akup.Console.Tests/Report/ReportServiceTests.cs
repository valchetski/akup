using Akup.Console.Report;
using Shouldly;

namespace Akup.Console.Tests.Report;

public class ReportServiceTests
{
    private readonly ReportService _reportService = new(null, null, null, null);

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
            Projects = [],
        };

        // act
        var act = () => _reportService.Report(config);

        // assert
        act.ShouldThrow<ReportException>();
    }
}
