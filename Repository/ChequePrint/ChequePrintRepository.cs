using AspNetCore.Reporting;
using ChequePrint.DTOs.ChequePrint;
using ChequePrint.Interfaces.ChequePrint;
using ClosedXML.Excel;
using System.Globalization;
using System.Text;
using System.Transactions;

namespace ChequePrint.Repository.ChequePrint
{
    public class ChequePrintRepository : IChequePrintRepository
    {
        private readonly IWebHostEnvironment _hostingEnvironment;

        public ChequePrintRepository(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        public async Task ChequePrintAttachmentUploadAsync(ChequePrintAttachmentUploadDTO model)
        {
            try
            {
                var transactionOptions = new TransactionOptions { Timeout = TimeSpan.FromMinutes(5), IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted };
                using (var transactionScope = new TransactionScope(TransactionScopeOption.Required, transactionOptions, TransactionScopeAsyncFlowOption.Enabled))
                {
                    if (model.Files.Count > 0)
                    {
                        foreach (var file in model.Files)
                        {
                            if (file != null)
                            {
                                var lastRow = 0;

                                var basePathInProj = Path.Combine(_hostingEnvironment.WebRootPath, "AdminRequestFileUpload");
                                bool basePathExists = Directory.Exists(basePathInProj);
                                if (!basePathExists) Directory.CreateDirectory(basePathInProj);

                                var fileName = Path.GetFileName(file.FileName);
                                var extension = Path.GetExtension(file.FileName);

                                if (extension != ".xls" && extension != ".xlsx")
                                {
                                    throw new Exception("Invalid Template");
                                }

                                var fileNameString = $"{fileName}{extension}";
                                var filePath = Path.Combine(basePathInProj, fileNameString);

                                if (!File.Exists(basePathInProj))
                                {
                                    using (var stream = new FileStream(filePath, FileMode.Create))
                                    {
                                        await file.CopyToAsync(stream);
                                    }
                                }

                                // Read excel
                                using var wbook = new XLWorkbook($"{basePathInProj}/{fileName}{extension}");
                                IXLWorksheet worksheet = wbook.Worksheet(1);
                                var ws = wbook.Worksheet(worksheet.Name);

                                var TempHeader = "Employee NameDateAmount";
                                var CreateTempHeader = "";

                                bool FirstRow = true;
                                string readRange = "1:1";
                                foreach (IXLRow row in ws.RowsUsed())
                                {
                                    if (FirstRow)
                                    {
                                        readRange = string.Format("{0}:{1}", 1, row.LastCellUsed().Address.ColumnNumber);
                                        foreach (IXLCell cell in row.Cells(readRange))
                                        {
                                            CreateTempHeader += cell.Value.ToString().Trim();
                                        }
                                        FirstRow = false;
                                    }
                                }

                                if (TempHeader != CreateTempHeader)
                                {
                                    throw new Exception("Invalid Template");
                                }

                                var lastRowUsed = ws.LastRowUsed();
                                lastRow = lastRowUsed.RowNumber();

                                if (lastRow < 2)
                                {
                                    throw new Exception("There is no records in the uploaded file");
                                }

                                var _checkPrintDataList = new List<CheckPrintDataDTO>();
                                for (var i = 2; i <= lastRow; i++)
                                {
                                    var _employeeName = ws.Cell($"A{i}").GetValue<string>().Trim();
                                    var _date = ws.Cell($"B{i}").GetValue<string>().Trim();
                                    var _amount = ws.Cell($"C{i}").GetValue<string>().Trim();

                                    var _checkPrintData = new CheckPrintDataDTO();
                                    _checkPrintData.EmployeeName = _employeeName;
                                    _checkPrintData.Date = _date;
                                    _checkPrintData.Amount = _amount;

                                    _checkPrintDataList.Add(_checkPrintData);
                                }

                                foreach (var item in _checkPrintDataList)
                                {
                                    // Parse the date extracted from Excel (e.g. "8/13/2026")
                                    if (!DateTime.TryParse(item.Date, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate))
                                    {
                                        // fallback for common Excel date formats if the default parse fails
                                        var formats = new[] { "M/d/yyyy", "MM/dd/yyyy", "d/M/yyyy", "dd/MM/yyyy", "yyyy-MM-dd" };
                                        if (!DateTime.TryParseExact(item.Date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate))
                                        {
                                            throw new Exception($"Invalid date format for employee '{item.EmployeeName}': '{item.Date}'");
                                        }
                                    }

                                    var yearDigits = parsedDate.Year.ToString("D4");
                                    var monthDigits = parsedDate.Month.ToString("D2");
                                    var dayDigits = parsedDate.Day.ToString("D2");

                                    string Year1 = yearDigits[0].ToString();
                                    string Year2 = yearDigits[1].ToString();
                                    string Year3 = yearDigits[2].ToString();
                                    string Year4 = yearDigits[3].ToString();
                                    string Month1 = monthDigits[0].ToString();
                                    string Month2 = monthDigits[1].ToString();
                                    string Date1 = dayDigits[0].ToString();
                                    string Date2 = dayDigits[1].ToString();

                                    // Parse the amount extracted from Excel (handles "10,000.00")
                                    if (!decimal.TryParse(item.Amount, NumberStyles.Number, CultureInfo.InvariantCulture, out var amountDecimal))
                                    {
                                        throw new Exception($"Invalid amount for employee '{item.EmployeeName}': '{item.Amount}'");
                                    }

                                    string Amount = amountDecimal.ToString("N2", CultureInfo.InvariantCulture);
                                    string _amountInWord = ConvertAmountToWords(amountDecimal);
                                    string _amountInWordSuffix = "Rupees Only";

                                    string mimetypeCheckPrint = "";
                                    int extensionCheckPrint = 1;

                                    var checkPrintLetterDetail = new List<CheckPrintDataSetDTO> { new CheckPrintDataSetDTO { EmployeeName = $"{item.EmployeeName}", Amount = $"{Amount}", Year1 = $"{Year1}", Year2 = $"{Year2}", Year3 = $"{Year3}", Year4 = $"{Year4}", Month1 = $"{Month1}", Month2 = $"{Month2}", Date1 = $"{Date1}", Date2 = $"{Date2}", AmountInWord = $"{_amountInWord} {_amountInWordSuffix}" } };
                                    // RDLC Report Path
                                    var reportRdlcPath = $"{_hostingEnvironment.WebRootPath}\\Report\\ChequePrint\\ChequePrint.rdlc";

                                    Dictionary<string, string> para = new Dictionary<string, string>();
                                    para.Add("prm", "RDLC Report");

                                    // Create LocalReport object and add data source
                                    LocalReport rpt = new LocalReport(reportRdlcPath);
                                    rpt.AddDataSource("dsChequePrint", checkPrintLetterDetail);

                                    // Render the report as PDF
                                    var reportResultLetter = rpt.Execute(RenderType.Pdf, extensionCheckPrint, para, mimetypeCheckPrint);

                                    // File name and save path
                                    string fileNameLetter = "LOFIN Fund Transfer Letter.pdf";
                                    var savePathLetter = $"{_hostingEnvironment.WebRootPath}\\ReportGenerate\\{fileNameLetter}";

                                    // Save the PDF file
                                    await System.IO.File.WriteAllBytesAsync(savePathLetter, reportResultLetter.MainStream);
                                }

                                if (System.IO.File.Exists(filePath))
                                {
                                    System.IO.File.Delete(filePath);
                                }

                                transactionScope.Complete();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private static readonly string[] _units = { "", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten",
                                        "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen" };
        private static readonly string[] _tens = { "", "", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety" };

        private static string ConvertAmountToWords(decimal amount)
        {
            var wholePart = (long)Math.Floor(amount);
            var fractionalPart = (int)Math.Round((amount - wholePart) * 100);

            var words = ConvertWholeNumberToWords(wholePart);

            if (fractionalPart > 0)
            {
                words += $" and {ConvertWholeNumberToWords(fractionalPart)} Cents";
            }

            return words;
        }

        private static string ConvertWholeNumberToWords(long number)
        {
            if (number == 0) return "Zero";

            var crore = number / 10000000;
            number %= 10000000;
            var lakh = number / 100000;
            number %= 100000;
            var thousand = number / 1000;
            number %= 1000;
            var hundred = number / 100;
            var remainder = number % 100;

            var sb = new StringBuilder();

            if (crore > 0) sb.Append($"{ConvertWholeNumberToWords(crore)} Crore ");
            if (lakh > 0) sb.Append($"{ConvertWholeNumberToWords(lakh)} Lakh ");
            if (thousand > 0) sb.Append($"{ConvertWholeNumberToWords(thousand)} Thousand ");
            if (hundred > 0) sb.Append($"{_units[hundred]} Hundred ");

            if (remainder > 0)
            {
                if (sb.Length > 0) sb.Append("and ");

                if (remainder < 20)
                {
                    sb.Append(_units[remainder]);
                }
                else
                {
                    sb.Append(_tens[remainder / 10]);
                    if (remainder % 10 > 0) sb.Append($"-{_units[remainder % 10]}");
                }
            }

            return sb.ToString().Trim();
        }
    }
}