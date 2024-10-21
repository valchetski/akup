using Akup.Console.Report;
using Akup.Git;
using Akup.Report;
using Akup.Report.Tokens;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OfficeOpenXml;

namespace Akup.Console;

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

        return exitCode;
    }

    public static void GenerateReport(IServiceProvider serviceProvider)
    {
        var reportService = serviceProvider.GetRequiredService<IReportService>();
        var reportConfig = serviceProvider.GetRequiredService<IOptions<ReportConfig>>().Value;
        reportService.Report(reportConfig);
    }

    public static IHostBuilder CreateHostBuilder(params string[] args)
    {
        return new HostBuilder()
            .ConfigureDefaults(args)
            .ConfigureLogging(
                x => x.AddSimpleConsole(o =>
                {
                    o.IncludeScopes = true;
                    o.SingleLine = true;
                }))
            .ConfigureServices((context, services) =>
                services
                    .Configure<ReportConfig>(context.Configuration)
                    .AddSingleton<LocalGitRepositoryProvider>()
                    .AddSingleton<IGitRepositoryService>(sp => new GitRepositoryService(
                        new Dictionary<RepositorySource, IGitRepositoryProvider>()
                        {
                            { RepositorySource.Local, sp.GetRequiredService<LocalGitRepositoryProvider>() },
                        }))
                    .AddSingleton<ITokensService<ExcelWorksheet>, XlsxTokensService>()
                    .AddSingleton<ITokensService<string>, StringTokensService>()
                    .AddSingleton<IAkupReportService, XlsxAkupReportService>()
                    .AddSingleton<IReportService, ReportService>());
    }
}