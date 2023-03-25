using OfficeOpenXml;
using Pkup.Report.Tokens;

namespace Pkup.Report
{
    public class XlsxPkupReportService : IPkupReportService
    {
        private readonly ITokensService<ExcelWorksheet> _tokensService;

        static XlsxPkupReportService()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        public XlsxPkupReportService(ITokensService<ExcelWorksheet> tokensService)
        {
            _tokensService = tokensService;
        }

        public byte[] GeneratePkupReport(string templatePath, string defaultDateFormat, PkupInfo pkupInfo)
        {
            var fileinfo = new FileInfo(templatePath);
            if (!fileinfo.Exists)
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

                _tokensService.ReplaceTokens(worksheet, pkupInfo, defaultDateFormat);

                excelPackage.SaveAs(outputStream);
            }

            return outputStream.ToArray();
        }
    }
}
