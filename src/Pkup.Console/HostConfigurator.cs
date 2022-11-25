using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Pkup.Console.Report;
using Pkup.Git;
using Pkup.Report;

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
                .AddSingleton<IPkupReportService, XlsxPkupReportService>()
                .AddSingleton<IReportService, ReportService>();

            var gitConfigSectionName = "Git";
            services
                .AddOptions<GitConfig>()
                .Bind(context.Configuration.GetSection(gitConfigSectionName))
                .Validate(x => x != null, $"\"{gitConfigSectionName}\" config is not specified")
                .Validate(x => x.RepositoriesSources != null, $"\"{gitConfigSectionName}:{nameof(GitConfig.RepositoriesSources)}\" config is not set");

            var reportConfigSectionName = "Report";
            services
                .AddOptions<ReportConfig>()
                .Bind(context.Configuration.GetSection(reportConfigSectionName))
                .Validate(x => x != null, $"\"{reportConfigSectionName}\" config is not specified")
                .Validate(x => x.TemplatePath != null, $"\"{reportConfigSectionName}:{nameof(ReportConfig.TemplatePath)}\" config is not set")
                .Validate(x => x.ReportPath != null, $"\"{reportConfigSectionName}:{nameof(ReportConfig.ReportPath)}\" config is not set");
        }
    }
}
