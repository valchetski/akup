using Microsoft.Extensions.Logging;
using Pkup.Git;
using Pkup.Report;
using Pkup.Report.Tokens;

namespace Pkup.Console.Report
{
    public class ReportService : IReportService
    {
        private readonly IGitRepositoryService _gitRepositoryService;
        private readonly IPkupReportService _pkupReportService;
        private readonly ITokensService<string> _tokensService;
        private readonly ILogger _logger;

        public ReportService(
            IGitRepositoryService gitRepositoryService,
            IPkupReportService pkupReportService,
            ITokensService<string> tokensService,
            ILogger<ReportService> logger)
        {
            _gitRepositoryService = gitRepositoryService;
            _pkupReportService = pkupReportService;
            _tokensService = tokensService;
            _logger = logger;
        }

        public void Report(PkupConfig config)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            if (config.Projects == null)
            {
                throw new ArgumentNullException(nameof(config), nameof(config.Projects));
            }

            if (config.AuthorName == null)
            {
                throw new ArgumentNullException(nameof(config), nameof(config.AuthorName));
            }

            if (config.TemplatePath == null)
            {
                throw new ArgumentNullException(nameof(config), nameof(config.TemplatePath));
            }

            if (config.ReportPath == null)
            {
                throw new ArgumentNullException(nameof(config), nameof(config.ReportPath));
            }

            if (config.FromDate > config.ToDate)
            {
                throw new ReportException($"{nameof(config.FromDate)} cannot be greater then {nameof(config.ToDate)}");
            }

            var workDetails = new List<WorkDetail>();
            foreach (var projectConfig in config.Projects)
            {
                _logger.LogInformation("Looking for Git repositories of {ProjectName} project", projectConfig.ProjectName);
                if (projectConfig.RepositoriesSources == null)
                {
                    throw new ReportException($"{nameof(projectConfig.RepositoriesSources)} are not set");
                }

                var repositoriesPaths = _gitRepositoryService.GetRepositoriesPaths(projectConfig.RepositoriesSources);
                if (!repositoriesPaths.Any())
                {
                    throw new FileNotFoundException($"No Git repositories found at {string.Join(", ", projectConfig.RepositoriesSources)}");
                }

                _logger.LogInformation("Searching commits in Git repositories of {ProjectName} project", projectConfig.ProjectName);
                var commits = _gitRepositoryService.GetCommits(repositoriesPaths, config.AuthorName, config.FromDate, config.ToDate);

                workDetails.AddRange(commits.Select(x => new WorkDetail()
                {
                    ProjectName = projectConfig.ProjectName,
                    Description = x.Message,
                    Url = x.Url,
                }));
            }

            _logger.LogInformation("Generating report");
            var pkupInfo = new PkupInfo()
            {
                Details = workDetails,
                Tokens = config.Tokens,
                FromDate = config.FromDate,
                ToDate = config.ToDate,
            };
            var bytes = _pkupReportService.GeneratePkupReport(config.TemplatePath, config.DefaultDateFormat, pkupInfo);
            var savePath = _tokensService.ReplaceTokens(config.ReportPath, pkupInfo, config.DefaultDateFormat);
            File.WriteAllBytes(savePath, bytes);
            _logger.LogInformation("Report can be found at: {ReportPath}", savePath);
        }
    }
}
