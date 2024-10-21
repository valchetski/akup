namespace Akup.Git
{
    public class GitRepositoryService : IGitRepositoryService
    {
        private readonly Dictionary<RepositorySource, IGitRepositoryProvider> _providers;

        public GitRepositoryService(Dictionary<RepositorySource, IGitRepositoryProvider> providers)
        {
            _providers = providers;
        }

        public string[] GetRepositoriesPaths(params string[] searchLocations)
        {
            var repositoriesPaths = new List<string>();
            foreach (var searchLocation in searchLocations)
            {
                repositoriesPaths.AddRange(GetProvider(searchLocation).GetRepositoriesPaths(searchLocation));
            }

            return repositoriesPaths.ToArray();
        }

        public CommitInfo[] GetCommits(string[] repositoriesPaths, string authorName, DateTimeOffset? fromDate, DateTimeOffset? toDate)
        {
            var commits = new List<CommitInfo>();
            foreach (var repositoryPath in repositoriesPaths)
            {
                commits.AddRange(GetProvider(repositoryPath).GetCommits(repositoryPath, authorName, fromDate, toDate));
            }

            return commits.OrderBy(x => x.Date).ToArray();
        }

        private IGitRepositoryProvider GetProvider(string location)
        {
            RepositorySource repositorySource = RepositorySource.Local;
            if (location.StartsWith("https://github.com"))
            {
                repositorySource = RepositorySource.GitHub;
            }

            return _providers[repositorySource];
        }
    }
}
