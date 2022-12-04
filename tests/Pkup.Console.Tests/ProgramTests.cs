using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using PKUP;

namespace Pkup.Console.Tests
{
    public class ProgramTests
    {
        [Fact]
        public void Program_Should_GenerateReport()
        {
            // arrange
            var services = Program.GetServices(hb => hb
                .ConfigureAppConfiguration(x => x.AddJsonFile("appsettings.tests.json")));

            // act
            Action act = () => Program.GenerateReport(services);

            // assert
            act.Should().NotThrow();
        }
    }
}