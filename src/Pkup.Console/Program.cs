using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Pkup.Console;
using Pkup.Console.Report;

namespace PKUP
{
    internal static class Program
    {
        static void Main()
        {
            var serviceProvider = HostConfigurator.CreateHostBuilder().Build().Services;

            var reportService = serviceProvider.GetRequiredService<IReportService>();
            var gitConfig = serviceProvider.GetRequiredService<IOptions<GitConfig>>().Value;
            var reportConfig = serviceProvider.GetRequiredService<IOptions<ReportConfig>>().Value;
            reportService.Report(
                gitConfig.RepositoriesSources!,
                gitConfig.AuthorName!,
                gitConfig.FromDate,
                gitConfig.ToDate,
                reportConfig.ProjectName!,
                reportConfig.TemplatePath!,
                reportConfig.ReportPath!,
                reportConfig.Tokens!);

            Console.WriteLine($"Press any key to exit...");
            Console.ReadKey();
        }
    }
}