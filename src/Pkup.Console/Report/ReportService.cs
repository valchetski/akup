using Microsoft.Extensions.Logging;
using Pkup.Git;
using Pkup.Report;
using Pkup.Report.Tokens;

namespace Pkup.Console.Report
{
    public class ReportService : IReportService
    {
        private readonly IGitRepositoryService _gitRepositoryService;
        private readonly IGitReportService _gitReportService;
        private readonly IPkupReportService _pkupReportService;
        private readonly ITokensService<string> _tokensService;
        private readonly ILogger _logger;

        public ReportService(
            IGitRepositoryService gitRepositoryService,
            IGitReportService gitReportService,
            IPkupReportService pkupReportService,
            ITokensService<string> tokensService,
            ILogger<ReportService> logger)
        {
            _gitRepositoryService = gitRepositoryService;
            _gitReportService = gitReportService;
            _pkupReportService = pkupReportService;
            _tokensService = tokensService;
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
            if (fromDate > toDate)
            {
                throw new ReportException($"{nameof(fromDate)} cannot be greater then {nameof(toDate)}");
            }

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
            var savePath = _tokensService.ReplaceTokens(reportPath, pkupInfo, defaultDateFormat);
            File.WriteAllBytes(savePath, bytes);
            _logger.LogInformation("Report can be found at: {ReportPath}", reportPath);
        }
    }
}
