using Flurl;
using LibGit2Sharp;

namespace Pkup.Git
{
    public class LocalGitRepositoryProvider : IGitRepositoryProvider
    {
        public CommitInfo[] GetCommits(string repositoryPath, string authorName, DateTimeOffset? fromDate, DateTimeOffset? toDate)
        {
            using var repo = new Repository(repositoryPath);
            return FindCommits(repo, authorName, fromDate, toDate)
                .Select(x => new CommitInfo()
                {
                    Date = x.Author.When,
                    Message = x.Message,
                    Url = GetCommitUrl(repo, x),
                })
                .ToArray();
        }

        public string[] GetRepositoriesPaths(string searchLocation)
        {
            return Directory.GetDirectories(searchLocation, ".git", SearchOption.AllDirectories);
        }

        private static Commit[] FindCommits(Repository repo, string? authorName = default, DateTimeOffset? from = default, DateTimeOffset? to = default)
        {
            var query = repo.Commits.AsEnumerable();

            if (!string.IsNullOrEmpty(authorName))
            {
                query = query.Where(x => x.Author.Name == authorName);
            }

            if (from != default)
            {
                query = query.Where(x => x.Author.When >= from);
            }

            if (to != default)
            {
                query = query.Where(x => x.Author.When <= to);
            }

            return query.ToArray();
        }

        private static string GetCommitUrl(Repository repo, Commit commit)
        {
            // Builds GitHub url
            return repo.Network.Remotes.First().Url.Replace(".git", string.Empty).AppendPathSegments("commit", commit.Id);
        }
    }
}
