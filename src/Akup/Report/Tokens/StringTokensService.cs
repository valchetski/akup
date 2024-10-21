namespace Akup.Report.Tokens
{
    public class StringTokensService : TokensService<string>
    {
        protected override string ReplaceTokenInternal(string data, string tokenKey, string tokenValue)
        {
            return Replace(data, tokenKey, tokenValue);
        }

        protected override string ReplaceDateTokenInternal(string data, string dateTokenKey, DateTimeOffset dateTokenValue, string defaultDateFormat)
        {
            if (HasDateToken(data, dateTokenKey))
            {
                return ReplaceDateToken(data, dateTokenKey, dateTokenValue, defaultDateFormat);
            }

            return data;
        }
    }
}
