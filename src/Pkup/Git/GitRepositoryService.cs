using Flurl;
using LibGit2Sharp;

namespace Pkup.Git
{
    public class GitRepositoryService : IGitRepositoryService
    {
        public Commit[] FindCommits(Repository repo, string? authorName = default, DateTimeOffset? from = default, DateTimeOffset? to = default)
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

        public string GetCommitUrl(Repository repo, Commit commit)
        {
            // Builds GitHub url
            return repo.Network.Remotes.First().Url.AppendPathSegments("commit", commit.Id);
        }
    }
}
