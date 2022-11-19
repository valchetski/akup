namespace Pkup.Console
{
    public class GitConfig
    {
        public string[]? RepositoriesPaths { get; set; }

        public string? AuthorName { get; set; }

        public DateTimeOffset? FromDate { get; set; }

        public DateTimeOffset? ToDate { get; set; }
    }
}
