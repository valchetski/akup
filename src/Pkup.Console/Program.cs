using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pkup.Console;
using Pkup.Console.Report;

namespace PKUP
{
    public static class Program
    {
        static int Main()
        {
            var exitCode = 0;
            var services = GetServices();
            var logger = services.GetRequiredService<ILoggerFactory>().CreateLogger(nameof(Program));
            try
            {
                GenerateReport(services);
            }
            catch(Exception ex)
            {
                logger.LogCritical(ex, "Unhandled exception occured");
                exitCode = 1;
            }

            logger.LogInformation("Press any key to exit...");
            Console.ReadKey();
            return exitCode;
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
            var reportConfig = serviceProvider.GetRequiredService<IOptions<PkupConfig>>().Value;
            reportService.Report(
                reportConfig.RepositoriesSources!,
                reportConfig.AuthorName!,
                reportConfig.FromDate,
                reportConfig.ToDate,
                reportConfig.ProjectName!,
                reportConfig.TemplatePath!,
                reportConfig.ReportPath!,
                reportConfig.DefaultDateFormat,
                reportConfig.Tokens!);
        }
    }
}