namespace Akup.Git;

public interface IGitRepositoryService
{
    CommitInfo[] GetCommits(string[] repositoriesPaths, string authorName, DateTimeOffset? fromDate, DateTimeOffset? toDate);

    string[] GetRepositoriesPaths(params string[] searchLocations);
}
