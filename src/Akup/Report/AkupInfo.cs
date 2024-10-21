namespace Akup.Report;

public class AkupInfo
{
    public List<WorkDetail>? Details { get; set; }

    public Dictionary<string, string>? Tokens { get; set; }

    public DateTimeOffset? FromDate { get; set; }

    public DateTimeOffset? ToDate { get; set; }
}
