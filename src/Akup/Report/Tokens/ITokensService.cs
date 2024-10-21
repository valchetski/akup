namespace Akup.Report.Tokens;

public interface ITokensService<T>
{
    T ReplaceTokens(T data, AkupInfo akupInfo, string defaultDateFormat);
}
