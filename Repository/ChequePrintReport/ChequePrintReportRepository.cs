using ChequePrint.Interfaces.ChequePrintReport;
using ChequePrint.ViewModels;
using ClosedXML.Excel;

namespace ChequePrint.Repository.ChequePrintReport
{
    public class ChequePrintReportRepository : IChequePrintReportRepository
    {
        private readonly IWebHostEnvironment _hostingEnvironment;

        public ChequePrintReportRepository(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        public async Task<List<ChequePrintReportViewModel>> GetAllAsync()
        {
            try
            {
                var _checkPrintTrackings = new List<ChequePrintReportViewModel>();

                // Define the Excel file path
                var excelDirectory = Path.Combine(_hostingEnvironment.WebRootPath, "CheckPrintTracking");
                var excelFilePath = Path.Combine(excelDirectory, "Check_Print_Tracking_v1.xlsx");

                // Check if the file exists
                if (!File.Exists(excelFilePath))
                {
                    return _checkPrintTrackings; // Return empty list if file doesn't exist
                }

                // Open the Excel file
                using (var workbook = new XLWorkbook(excelFilePath))
                {
                    var worksheet = workbook.Worksheet(1); // Get the first worksheet

                    // Get the last row with data
                    var lastRow = worksheet.LastRowUsed()?.RowNumber() ?? 1;

                    // Start from row 2 (skip header row)
                    for (int row = 2; row <= lastRow; row++)
                    {
                        // Skip empty rows
                        if (string.IsNullOrWhiteSpace(worksheet.Cell(row, 1).GetValue<string>()))
                        {
                            continue;
                        }

                        var chequePrint = new ChequePrintReportViewModel();
                        chequePrint.ChequeName = worksheet.Cell(row, 1).GetValue<string>() ?? "N/A";
                        chequePrint.Date = worksheet.Cell(row, 2).GetDateTime();
                        chequePrint.Amount = Convert.ToDouble(worksheet.Cell(row, 3).GetValue<decimal>());
                        chequePrint.PrintedOn = worksheet.Cell(row, 4).GetDateTime();

                        _checkPrintTrackings.Add(chequePrint);
                    }
                }

                return _checkPrintTrackings;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error reading cheque print tracking data: {ex.Message}", ex);
            }
        }
    }
}