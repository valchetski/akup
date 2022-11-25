namespace Pkup.Git
{
    public interface IGitReportService
    {
        CommitInfo[] GetCommits(string[] repositoriesPaths, string authorName, DateTimeOffset? fromDate, DateTimeOffset? toDate);
    }
}
