namespace Pkup.Report
{
    public interface IPkupReportService
    {
        byte[] GeneratePkupReport(string templatePath, PkupInfo pkupInfo);
    }
}
