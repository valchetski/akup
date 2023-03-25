using Flurl;
using LibGit2Sharp;

namespace Pkup.Git
{
    public class GitReportService : IGitReportService
    {
        private readonly IGitRepositoryService _gitRepositoryService;

        public GitReportService(IGitRepositoryService gitRepositoryService)
        {
            _gitRepositoryService = gitRepositoryService;
        }

        public CommitInfo[] GetCommits(string[] repositoriesPaths, string authorName, DateTimeOffset? fromDate, DateTimeOffset? toDate)
        {
            var commits = new List<CommitInfo>();
            foreach (var repository in repositoriesPaths)
            {
                using var repo = new Repository(repository);
                commits.AddRange(_gitRepositoryService
                    .FindCommits(repo, authorName, fromDate, toDate)
                    .Select(x => new CommitInfo()
                    {
                        Date = x.Author.When,
                        Message = x.Message,
                        Url = GetCommitUrl(repo, x),
                    }));
            }

            return commits.OrderBy(x => x.Date).ToArray();
        }

        private static string GetCommitUrl(Repository repo, Commit commit)
        {
            // Builds GitHub url
            return repo.Network.Remotes.First().Url.Replace(".git", string.Empty).AppendPathSegments("commit", commit.Id);
        }
    }
}
