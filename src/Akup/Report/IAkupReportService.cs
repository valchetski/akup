namespace Akup.Report;

public interface IAkupReportService
{
    byte[] GenerateAkupReport(string templatePath, string defaultDateFormat, AkupInfo akupInfo);
}
