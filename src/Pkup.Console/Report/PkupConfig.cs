namespace Pkup.Console.Report
{
    public class PkupConfig
    {
        public PkupConfig()
            : this(null, null, null, null, null, null, null)
        {
        }

        public PkupConfig(
            string? authorName,
            DateTimeOffset? fromDate,
            DateTimeOffset? toDate,
            string? templatePath,
            string? reportPath,
            Dictionary<string, string>? tokens,
            ProjectConfig[]? projects,
            string defaultDateFormat = "dd MMMM")
        {
            AuthorName = authorName;
            FromDate = fromDate;
            ToDate = toDate;
            TemplatePath = templatePath;
            ReportPath = reportPath;
            DefaultDateFormat = defaultDateFormat;
            Tokens = tokens;
            Projects = projects;
        }

        public string? AuthorName { get; set; }

        public DateTimeOffset? FromDate { get; set; }

        public DateTimeOffset? ToDate { get; set; }

        public string? TemplatePath { get; set; }

        public string? ReportPath { get; set; }

        public string DefaultDateFormat { get; set; }

        public Dictionary<string, string>? Tokens { get; set; }

        public ProjectConfig[]? Projects { get; set; }
    }
}
