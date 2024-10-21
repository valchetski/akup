using OfficeOpenXml;

namespace Akup.Report.Tokens;

public class XlsxTokensService : TokensService<ExcelWorksheet>
{
    protected override ExcelWorksheet ReplaceTokenInternal(ExcelWorksheet data, string tokenKey, string tokenValue)
    {
        var cellsWithToken = data.Cells
                    .Where(x => x.GetValue<string>() != null && x.GetValue<string>().Contains(tokenKey))
                    .ToArray();
        foreach (var cellWithToken in cellsWithToken)
        {
            cellWithToken.Value = Replace(cellWithToken.GetValue<string>(), tokenKey, tokenValue);
        }

        return data;
    }

    protected override ExcelWorksheet ReplaceDateTokenInternal(ExcelWorksheet data, string dateTokenKey, DateTimeOffset dateTokenValue, string defaultDateFormat)
    {
        var cellsWithToken = data.Cells
                    .Where(x => HasDateToken(x.GetValue<string>(), dateTokenKey))
                    .ToArray();
        foreach (var cellWithToken in cellsWithToken)
        {
            cellWithToken.Value = ReplaceDateToken(cellWithToken.GetValue<string>(), dateTokenKey, dateTokenValue, defaultDateFormat);
        }

        return data;
    }
}
