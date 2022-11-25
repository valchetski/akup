namespace Pkup.Console.Report
{
    public class GitConfig
    {
        public string[]? RepositoriesSources { get; set; }

        public string? AuthorName { get; set; }

        public DateTimeOffset? FromDate { get; set; }

        public DateTimeOffset? ToDate { get; set; }
    }
}
