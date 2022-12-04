using Microsoft.Extensions.Logging;
using Pkup.Git;
using Pkup.Report;

namespace Pkup.Console.Report
{
    public class ReportService : IReportService
    {
        private readonly IGitRepositoryService _gitRepositoryService;
        private readonly IGitReportService _gitReportService;
        private readonly IPkupReportService _pkupReportService;
        private readonly ILogger _logger;

        public ReportService(
            IGitRepositoryService gitRepositoryService,
            IGitReportService gitReportService,
            IPkupReportService pkupReportService,
            ILogger<ReportService> logger)
        {
            _gitRepositoryService = gitRepositoryService;
            _gitReportService = gitReportService;
            _pkupReportService = pkupReportService;
            _logger = logger;
        }

        public void Report(
            string[] repositoriesSources,
            string authorName,
            DateTimeOffset? fromDate,
            DateTimeOffset? toDate,
            string projectName,
            string templatePath,
            string reportPath,
            string defaultDateFormat,
            Dictionary<string, string> tokens)
        {
            _logger.LogInformation("Looking for Git repositories");
            var repositoriesPaths = _gitRepositoryService.GetRepositories(repositoriesSources);
            if (!repositoriesPaths.Any())
            {
                throw new FileNotFoundException($"No Git repositories found at {string.Join(", ", repositoriesSources) }");
            }

            _logger.LogInformation("Searching commits");
            var commits = _gitReportService.GetCommits(repositoriesPaths, authorName, fromDate, toDate);

            _logger.LogInformation("Generating report");
            var pkupInfo = new PkupInfo()
            {
                Details = commits.Select(x => new WorkDetail()
                {
                    ProjectName = projectName,
                    Description = x.Message,
                    Url = x.Url
                }).ToList(),
                Tokens = tokens,
                FromDate = fromDate,
                ToDate = toDate,
            };
            var bytes = _pkupReportService.GeneratePkupReport(templatePath, defaultDateFormat, pkupInfo);
            File.WriteAllBytes(reportPath, bytes);
            _logger.LogInformation("Report can be found at: {ReportPath}", reportPath);
        }
    }
}
