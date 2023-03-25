using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OfficeOpenXml;
using Pkup.Console.Report;
using Pkup.Git;
using Pkup.Report;
using Pkup.Report.Tokens;

namespace PKUP
{
    public static class Program
    {
        public static int Main()
        {
            var exitCode = 0;
            var services = CreateHostBuilder().Build().Services;
            var logger = services.GetRequiredService<ILoggerFactory>().CreateLogger(nameof(Program));
            try
            {
                GenerateReport(services);
            }
            catch (Exception ex)
            {
                logger.LogCritical(ex, "Unhandled exception occured");
                exitCode = 1;
            }

            logger.LogInformation("Press any key to exit...");
            Console.ReadKey();
            return exitCode;
        }

        public static void GenerateReport(IServiceProvider serviceProvider)
        {
            var reportService = serviceProvider.GetRequiredService<IReportService>();
            var reportConfig = serviceProvider.GetRequiredService<IOptions<PkupConfig>>().Value;
            reportService.Report(reportConfig);
        }

        public static IHostBuilder CreateHostBuilder()
        {
            return new HostBuilder()
                .ConfigureAppConfiguration(
                    x => x.SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false)
                    .AddJsonFile("appsettings.json.user", optional: true))
                .ConfigureLogging(
                    x => x.AddSimpleConsole(o =>
                    {
                        o.IncludeScopes = true;
                        o.SingleLine = true;
                    }))
                .ConfigureServices((context, services) =>
                    services
                        .Configure<PkupConfig>(context.Configuration)
                        .AddSingleton<IGitRepositoryService, GitRepositoryService>()
                        .AddSingleton<IGitReportService, GitReportService>()
                        .AddSingleton<ITokensService<ExcelWorksheet>, XlsxTokensService>()
                        .AddSingleton<ITokensService<string>, StringTokensService>()
                        .AddSingleton<IPkupReportService, XlsxPkupReportService>()
                        .AddSingleton<IReportService, ReportService>());
        }
    }
}