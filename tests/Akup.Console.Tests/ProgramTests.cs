using Microsoft.Extensions.Hosting;
using Shouldly;

namespace Akup.Console.Tests;

public class ProgramTests
{
    [Fact]
    public void Program_Should_GenerateReport()
    {
        // arrange
        var services = GetServices();

        // act
        var act = () => Program.GenerateReport(services);

        // assert
        act.ShouldNotThrow();
    }

    private static IServiceProvider GetServices()
    {
        return Program
            .CreateHostBuilder()
            .UseEnvironment("Tests")
            .Build().Services;
    }
}