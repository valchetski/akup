namespace Pkup.Console.Report
{
    public class ReportConfig
    {
        public string? ProjectName { get; set; }

        public string? TemplatePath { get; set; }

        public string? ReportPath { get; set; }

        public Dictionary<string, string>? Tokens { get; set; }
    }
}
