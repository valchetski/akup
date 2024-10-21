using System.Text.RegularExpressions;

namespace Akup.Report.Tokens;

public abstract class TokensService<T> : ITokensService<T>
{
    protected const string DateRegexGroupName = "format";

    public T ReplaceTokens(T data, AkupInfo akupInfo, string defaultDateFormat)
    {
        if (akupInfo.Tokens != null)
        {
            foreach (var tokenKeyValue in akupInfo.Tokens)
            {
                var tokenKey = $"{{{tokenKeyValue.Key}}}";
                data = ReplaceTokenInternal(data, tokenKey, tokenKeyValue.Value);
            }
        }

        return ReplaceDateTokens(data, akupInfo, defaultDateFormat);
    }

    protected abstract T ReplaceTokenInternal(T data, string tokenKey, string tokenValue);

    protected abstract T ReplaceDateTokenInternal(T data, string dateTokenKey, DateTimeOffset dateTokenValue, string defaultDateFormat);

    protected string Replace(string data, string tokenKey, string tokenValue)
    {
        return data.Replace(tokenKey, tokenValue);
    }

    protected string ReplaceDateToken(string data, string dateTokenKey, DateTimeOffset dateTokenValue, string defaultDateFormat)
    {
        var match = Regex.Match(data, dateTokenKey);
        var dateFormat = match.Groups[DateRegexGroupName].Value;
        if (string.IsNullOrEmpty(dateFormat))
        {
            dateFormat = defaultDateFormat;
        }

        return Replace(data, match.Value, dateTokenValue.ToString(dateFormat));
    }

    protected bool HasDateToken(string data, string dateTokenKey)
    {
        return data != null && Regex.IsMatch(data, dateTokenKey);
    }

    private T ReplaceDateTokens(T data, AkupInfo akupInfo, string defaultDateFormat)
    {
        data = ReplaceDateToken(data, "FromDate", akupInfo.FromDate, defaultDateFormat);

        return ReplaceDateToken(data, "ToDate", akupInfo.ToDate, defaultDateFormat);
    }

    private T ReplaceDateToken(T data, string dateTokenKey, DateTimeOffset? date, string defaultDateFormat)
    {
        if (date == null)
        {
            return data;
        }

        var dateToken = @$"{{{dateTokenKey}:?(?<{DateRegexGroupName}>[^}}]*)}}";
        return ReplaceDateTokenInternal(data, dateToken, date.Value, defaultDateFormat);
    }
}
