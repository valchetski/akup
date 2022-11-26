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
            Dictionary<string, string>? replaceTokens = null;
            if (tokens != null)
            {
                replaceTokens = new Dictionary<string, string>(tokens);

                if (fromDate.HasValue)
                {
                    replaceTokens.Add("StartDate", fromDate.Value.ToString("dd MMMM"));
                }

                if (toDate.HasValue)
                {
                    replaceTokens.Add("EndDate", toDate.Value.ToString("dd MMMM"));
                }
            }
            var pkupInfo = new PkupInfo()
            {
                Details = commits.Select(x => new WorkDetail()
                {
                    ProjectName = projectName,
                    Description = x.Message,
                    Url = x.Url
                }).ToList(),
                Tokens = replaceTokens,
            };
            var bytes = _pkupReportService.GeneratePkupReport(templatePath, pkupInfo);
            File.WriteAllBytes(reportPath, bytes);
            _logger.LogInformation("Report can be found at: {ReportPath}", reportPath);
        }
    }
}
