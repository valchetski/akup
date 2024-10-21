using Akup.Git;
using Akup.Report;
using Akup.Report.Tokens;
using Microsoft.Extensions.Logging;

namespace Akup.Console.Report;

public class ReportService(
    IGitRepositoryService gitRepositoryService,
    IAkupReportService akupReportService,
    ITokensService<string> tokensService,
    ILogger<ReportService> logger) : IReportService
{
    private readonly IGitRepositoryService _gitRepositoryService = gitRepositoryService;
    private readonly IAkupReportService _akupReportService = akupReportService;
    private readonly ITokensService<string> _tokensService = tokensService;
    private readonly ILogger _logger = logger;

    public void Report(ReportConfig config)
    {
        ArgumentNullException.ThrowIfNull(config);

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
            if (repositoriesPaths.Length == 0)
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
        var akupInfo = new AkupInfo()
        {
            Details = workDetails,
            Tokens = config.Tokens,
            FromDate = config.FromDate,
            ToDate = config.ToDate,
        };
        var bytes = _akupReportService.GenerateAkupReport(config.TemplatePath, config.DefaultDateFormat, akupInfo);
        var savePath = _tokensService.ReplaceTokens(config.ReportPath, akupInfo, config.DefaultDateFormat);
        if (!Path.IsPathRooted(savePath))
        {
            savePath = Path.GetFullPath(savePath, AppDomain.CurrentDomain.BaseDirectory);
        }

        File.WriteAllBytes(savePath, bytes);
        _logger.LogInformation("Report can be found at: {ReportPath}", savePath);
    }
}
