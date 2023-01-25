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
            // act
            Action act = () => _reportService.Report(null, null, DateTimeOffset.Now.AddDays(1), DateTimeOffset.Now, null, null, null, null, null);

            // assert
            act.Should().ThrowExactly<ReportException>();
        }
    }
}
