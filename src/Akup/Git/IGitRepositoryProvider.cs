namespace Akup.Git
{
    public interface IGitRepositoryProvider
    {
        string[] GetRepositoriesPaths(string searchLocation);

        CommitInfo[] GetCommits(string repositoryPath, string authorName, DateTimeOffset? fromDate, DateTimeOffset? toDate);
    }
}
