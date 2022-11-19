using LibGit2Sharp;

namespace Pkup.Git
{
    public interface IGitRepositoryService
    {
        Commit[] FindCommits(Repository repo, string? authorName = default, DateTimeOffset? from = default, DateTimeOffset? to = default);

        string GetCommitUrl(Repository repo, Commit commit);
    }
}
