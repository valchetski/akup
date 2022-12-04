using OfficeOpenXml;
using System.Text.RegularExpressions;

namespace Pkup.Report
{
    public class XlsxPkupReportService : IPkupReportService
    {
        static XlsxPkupReportService()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        public byte[] GeneratePkupReport(string templatePath, string defaultDateFormat, PkupInfo pkupInfo)
        {
            var fileinfo = new FileInfo(templatePath);
            if (!fileinfo.Exists )
            {
                throw new FileNotFoundException(templatePath, templatePath);
            }

            var outputStream = new MemoryStream();
            using (var excelPackage = new ExcelPackage(fileinfo))
            {
                var worksheet = excelPackage.Workbook.Worksheets[0];
                var table = worksheet.Tables[0];
                var start = table.Address.Start;
                var end = table.Address.End;

                var rowIndex = start.Row + 1;
                var columnIndex = start.Column;
                if (pkupInfo.Details != null)
                {
                    foreach (var workDetail in pkupInfo.Details)
                    {
                        if (rowIndex > end.Row)
                        {
                            table.AddRow();

                            var previousNumber = worksheet.Cells[rowIndex - 1, columnIndex].GetValue<int>();
                            worksheet.Cells[rowIndex, columnIndex].Value = previousNumber + 1;
                        }

                        worksheet.Cells[rowIndex, columnIndex + 1].Value = workDetail.ProjectName;

                        var descriptionCell = worksheet.Cells[rowIndex, columnIndex + 2];
                        if (workDetail.Url != null)
                        {
                            descriptionCell.Hyperlink = new Uri(workDetail.Url);
                        }
                        descriptionCell.Value = workDetail.Description;

                        worksheet.Row(rowIndex).CustomHeight = false;

                        rowIndex++;
                    }
                }

                ReplaceTokens(worksheet, pkupInfo, defaultDateFormat);

                excelPackage.SaveAs(outputStream);
            }

            return outputStream.ToArray();
        }

        private static void ReplaceTokens(ExcelWorksheet worksheet, PkupInfo pkupInfo, string defaultDateFormat)
        {
            if (pkupInfo.Tokens != null)
            {
                foreach (var tokenKeyValue in pkupInfo.Tokens)
                {
                    var token = $"{{{tokenKeyValue.Key}}}";
                    var cellsWithToken = worksheet.Cells
                        .Where(x => x.GetValue<string>() != null && x.GetValue<string>().Contains(token))
                        .ToArray();
                    foreach (var cellWithToken in cellsWithToken)
                    {
                        cellWithToken.Value = cellWithToken.GetValue<string>().Replace(token, tokenKeyValue.Value);
                    }
                }
            }

            ReplaceDateTokens(worksheet, pkupInfo, defaultDateFormat);
        }

        private static void ReplaceDateTokens(ExcelWorksheet worksheet, PkupInfo pkupInfo, string defaultDateFormat)
        {
            ReplaceDateToken(worksheet, "StartDate", pkupInfo.FromDate, defaultDateFormat);

            ReplaceDateToken(worksheet, "EndDate", pkupInfo.ToDate, defaultDateFormat);
        }

        private static void ReplaceDateToken(ExcelWorksheet worksheet, string dateTokenName, DateTimeOffset? date, string defaultDateFormat)
        {
            if (date == null)
            {
                return;
            }

            var groupName = "format";
            var dateToken = @$"{{{dateTokenName}:?(?<{groupName}>[^}}]*)}}";
            var cellsWithToken = worksheet.Cells
                        .Where(x => x.GetValue<string>() != null && Regex.IsMatch(x.GetValue<string>(), dateToken))
                        .ToArray();
            foreach (var cellWithToken in cellsWithToken)
            {
                var cellValue = cellWithToken.GetValue<string>();
                var match = Regex.Match(cellValue, dateToken);
                var dateFormat = match.Groups[groupName].Value;
                if(string.IsNullOrEmpty(dateFormat))
                {
                    dateFormat = defaultDateFormat;
                }

                cellWithToken.Value = cellWithToken.GetValue<string>().Replace(match.Value, date.Value.ToString(dateFormat));
            }
        }
    }
}
