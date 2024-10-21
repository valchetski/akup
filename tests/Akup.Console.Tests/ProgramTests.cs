using FluentAssertions;
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

    private static IServiceProvider GetServices()
    {
        return Program
            .CreateHostBuilder()
            .UseEnvironment("Tests")
            .Build().Services;
    }
}