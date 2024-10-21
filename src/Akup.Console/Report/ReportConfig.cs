namespace Akup.Console.Report
{
    public class ReportConfig
    {
        public string? AuthorName { get; set; }

        public DateTimeOffset? FromDate { get; set; }

        public DateTimeOffset? ToDate { get; set; }

        public string? TemplatePath { get; set; }

        public string? ReportPath { get; set; }

        public string DefaultDateFormat { get; set; } = "dd MMMM";

        public Dictionary<string, string>? Tokens { get; set; }

        public ProjectConfig[]? Projects { get; set; }
    }
}
