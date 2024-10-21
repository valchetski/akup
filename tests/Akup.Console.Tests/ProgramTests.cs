using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Akup.Console.Tests;

public class ProgramTests
{
    [Fact]
    public void Program_Should_GenerateReport()
    {
        // arrange
        var services = GetServices();

        // act
        Action act = () => Program.GenerateReport(services);

        // assert
        act.Should().NotThrow();
    }

    private static IServiceProvider GetServices(Dictionary<string, string> configOverride = null)
    {
        return Program
            .CreateHostBuilder()
            .ConfigureAppConfiguration(config =>
            {
                config.AddJsonFile("appsettings.tests.json");
                if (configOverride != null)
                {
                    config.AddInMemoryCollection(configOverride);
                }
            })
            .Build().Services;
    }
}