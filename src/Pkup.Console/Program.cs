using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Pkup.Console;
using Pkup.Console.Report;

namespace PKUP
{
    public static class Program
    {
        static void Main()
        {
            GenerateReport(GetServices());

            Console.WriteLine($"Press any key to exit...");
            Console.ReadKey();
        }

        public static IServiceProvider GetServices(Func<IHostBuilder, IHostBuilder>? customize = null)
        {
            var hostBuilder = HostConfigurator.CreateHostBuilder();

            if (customize != null)
            {
                hostBuilder = customize(hostBuilder);
            }

            return hostBuilder.Build().Services;
        }

        public static void GenerateReport(IServiceProvider serviceProvider)
        {
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
                reportConfig.DefaultDateFormat,
                reportConfig.Tokens!);
        }
    }
}