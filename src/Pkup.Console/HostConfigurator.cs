using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using Pkup.Console.Report;
using Pkup.Git;
using Pkup.Report;
using Pkup.Report.Tokens;

namespace Pkup.Console
{
    public static class HostConfigurator
    {
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
                .ConfigureServices(ConfigureServices);
        }

        private static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
        {
            services
                .AddSingleton<IGitRepositoryService, GitRepositoryService>()
                .AddSingleton<IGitReportService, GitReportService>()
                .AddSingleton<ITokensService<ExcelWorksheet>, XlsxTokensService>()
                .AddSingleton<ITokensService<string>, StringTokensService>()
                .AddSingleton<IPkupReportService, XlsxPkupReportService>()
                .AddSingleton<IReportService, ReportService>();

            services
                .AddOptions<PkupConfig>()
                .Bind(context.Configuration)
                .Validate(x => x.RepositoriesSources != null, $"\"{nameof(PkupConfig.RepositoriesSources)}\" config is not set")
                .Validate(x => x.TemplatePath != null, $"\"{nameof(PkupConfig.TemplatePath)}\" config is not set")
                .Validate(x => x.ReportPath != null, $"\"{nameof(PkupConfig.ReportPath)}\" config is not set");
        }
    }
}
