namespace Pkup.Report
{
    public interface IPkupReportService
    {
        byte[] GeneratePkupReport(string templatePath, string defaultDateFormat, PkupInfo pkupInfo);
    }
}
