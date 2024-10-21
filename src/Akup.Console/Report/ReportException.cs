namespace Akup.Console.Report;

public class ReportException : Exception
{
    public ReportException()
    {
    }

    public ReportException(string message)
        : base(message)
    {
    }
}
