using OfficeOpenXml;
using System.Linq;

namespace Pkup.Report
{
    public class XlsxPkupReportService : IPkupReportService
    {
        static XlsxPkupReportService()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        public byte[] GeneratePkupReport(string templatePath, PkupInfo pkupInfo)
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

                excelPackage.SaveAs(outputStream);
            }

            return outputStream.ToArray();
        }
    }
}
