namespace Pkup.Report.Tokens
{
    public interface ITokensService<T>
    {
        T ReplaceTokens(T data, PkupInfo pkupInfo, string defaultDateFormat);
    }
}
