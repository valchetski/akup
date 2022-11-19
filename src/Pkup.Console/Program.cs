using LibGit2Sharp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pkup.Console;
using Pkup.Git;
using Pkup.Report;

namespace PKUP
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // TODO:: Maybe move to Startup.cs?
            var services = new ServiceCollection();
            services
                .AddSingleton<IGitRepositoryService, GitRepositoryService>()
                .AddSingleton<IPkupReportService, XlsxPkupReportService>();
            var serviceProvider = services.BuildServiceProvider();

            var gitRepositoryService = serviceProvider.GetRequiredService<IGitRepositoryService>();

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile("appsettings.json.user", optional: true);
            IConfiguration config = builder.Build();

            var gitConfigSectionName = "Git";
            var gitConfig = config.GetSection(gitConfigSectionName).Get<GitConfig>();
            if (gitConfig == null )
            {
                throw new NullReferenceException($"\"{gitConfigSectionName}\" config is not specified");
            }
            if (gitConfig.RepositoriesPaths == null)
            {
                throw new NullReferenceException($"\"{gitConfigSectionName}:{nameof(gitConfig.RepositoriesPaths)}\" config is not set");
            }

            var reportConfigSectionName = "Report";
            var reportConfig = config.GetSection(reportConfigSectionName).Get<ReportConfig>();
            if (reportConfig == null)
            {
                throw new NullReferenceException($"\"{reportConfigSectionName}\" config is not specified");
            }
            if (reportConfig.TemplatePath == null)
            {
                throw new NullReferenceException($"\"{reportConfigSectionName}:{nameof(reportConfig.TemplatePath)}\" config is not set");
            }
            if (reportConfig.ReportPath == null)
            {
                throw new NullReferenceException($"\"{reportConfigSectionName}:{nameof(reportConfig.ReportPath)}\" config is not set");
            }

            Console.WriteLine("Looking for Git repositories");

            var commits = new List<CommitInfo>();
            var repositoriesPaths = new List<string>();
            foreach (var repositoryPath in gitConfig.RepositoriesPaths)
            {
                repositoriesPaths.AddRange(Directory.GetDirectories(repositoryPath, ".git", SearchOption.AllDirectories));
            }

            Console.WriteLine("Searching commits");

            foreach (var repository in repositoriesPaths)
            {
                using var repo = new Repository(repository);
                commits.AddRange(gitRepositoryService
                    .FindCommits(repo, gitConfig.AuthorName, gitConfig.FromDate, gitConfig.ToDate)
                    .Select(x => new CommitInfo()
                    {
                        Date = x.Author.When,
                        Message = x.Message,
                        Url = gitRepositoryService.GetCommitUrl(repo, x)
                    }));
            }
            commits = commits.OrderBy(x => x.Date).ToList();

            var pkupInfo = new PkupInfo()
            {
                Details = commits.Select(x => new WorkDetail()
                {
                    ProjectName = reportConfig.ProjectName,
                    Description = x.Message,
                    Url = x.Url
                }).ToList()
            };

            Console.WriteLine("Generating report");

            var reportService = serviceProvider.GetRequiredService<IPkupReportService>();
            var bytes = reportService.GeneratePkupReport(reportConfig.TemplatePath, pkupInfo);
            File.WriteAllBytes(reportConfig.ReportPath, bytes);

            Console.WriteLine("Done");
        }
    }
}