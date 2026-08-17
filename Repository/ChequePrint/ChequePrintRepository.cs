using AspNetCore.Reporting;
using ChequePrint.DTOs.ChequePrint;
using ChequePrint.Interfaces.ChequePrint;
using ClosedXML.Excel;
using System.Globalization;
using System.IO.Compression;
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

        public async Task<(byte[] Content, string FileName)> ChequePrintAsync(CheckPrintDTO model)
        {
            try
            {
                if (model.PaymentMethod == (byte)PaymentMethod.CASH)
                {
                    var _res = await CashChequePrintAsync(model);
                    return _res;
                }
                else if (model.PaymentMethod == (byte)PaymentMethod.CREDIT)
                {
                    var _res = await CashChequePrintAsync(model);
                    return _res;
                }
                else
                {
                    throw new Exception($"Unsupported payment method: {model.PaymentMethod}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error generating cheque print: {ex.Message}", ex);
            }
        }

        public async Task<(byte[] Content, string FileName)> CashChequePrintAsync(CheckPrintDTO model)
        {
            try
            {
                var yearDigits = model.Date.Year.ToString("D4");
                var monthDigits = model.Date.Month.ToString("D2");
                var dayDigits = model.Date.Day.ToString("D2");

                string Year1 = yearDigits[0].ToString();
                string Year2 = yearDigits[1].ToString();
                string Year3 = yearDigits[2].ToString();
                string Year4 = yearDigits[3].ToString();
                string Month1 = monthDigits[0].ToString();
                string Month2 = monthDigits[1].ToString();
                string Date1 = dayDigits[0].ToString();
                string Date2 = dayDigits[1].ToString();

                var amountDecimal = Convert.ToDecimal(model.Amount);
                string Amount = amountDecimal.ToString("N2", CultureInfo.InvariantCulture);
                string _amountInWord = ConvertAmountToWords(amountDecimal);

                string mimetypeCheckPrint = "";
                int extensionCheckPrint = 1;

                var checkPrintLetterDetail = new List<CheckPrintDataSetDTO> {
                        new CheckPrintDataSetDTO {
                            EmployeeName = model.ChequeName,
                            Amount = Amount,
                            Year1 = Year1,
                            Year2 = Year2,
                            Year3 = Year3,
                            Year4 = Year4,
                            Month1 = Month1,
                            Month2 = Month2,
                            Date1 = Date1,
                            Date2 = Date2,
                            AmountInWord = _amountInWord
                        }
                    };

                var reportRdlcPath = $"{_hostingEnvironment.WebRootPath}\\Report\\ChequePrint\\ChequePrint.rdlc";

                Dictionary<string, string> para = new Dictionary<string, string>();
                para.Add("prm", "RDLC Report");

                LocalReport rpt = new LocalReport(reportRdlcPath);
                rpt.AddDataSource("dsChequePrint", checkPrintLetterDetail);

                var reportResultLetter = rpt.Execute(RenderType.Pdf, extensionCheckPrint, para, mimetypeCheckPrint);

                var safeName = string.Join("_", model.ChequeName.Split(Path.GetInvalidFileNameChars()));
                var dateForFileName = model.Date.ToString("yyyy_MM_dd");
                var fileName = $"Cheque_{safeName}_{dateForFileName}.pdf";

                return (reportResultLetter.MainStream, fileName);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error generating cheque print: {ex.Message}", ex);
            }
        }

        public async Task<(byte[] Content, string FileName)> CreditChequePrintAsync(CheckPrintDTO model)
        {
            try
            {
                var yearDigits = model.Date.Year.ToString("D4");
                var monthDigits = model.Date.Month.ToString("D2");
                var dayDigits = model.Date.Day.ToString("D2");

                string Year1 = yearDigits[0].ToString();
                string Year2 = yearDigits[1].ToString();
                string Year3 = yearDigits[2].ToString();
                string Year4 = yearDigits[3].ToString();
                string Month1 = monthDigits[0].ToString();
                string Month2 = monthDigits[1].ToString();
                string Date1 = dayDigits[0].ToString();
                string Date2 = dayDigits[1].ToString();

                var amountDecimal = Convert.ToDecimal(model.Amount);
                string Amount = amountDecimal.ToString("N2", CultureInfo.InvariantCulture);
                string _amountInWord = ConvertAmountToWords(amountDecimal);

                string mimetypeCheckPrint = "";
                int extensionCheckPrint = 1;

                var checkPrintLetterDetail = new List<CheckPrintDataSetDTO> {
                        new CheckPrintDataSetDTO {
                            Amount = Amount,
                            Year1 = Year1,
                            Year2 = Year2,
                            Year3 = Year3,
                            Year4 = Year4,
                            Month1 = Month1,
                            Month2 = Month2,
                            Date1 = Date1,
                            Date2 = Date2,
                            AmountInWord = _amountInWord,
                            ChequeName = model.ChequeName
                        }
                    };

                var reportRdlcPath = $"{_hostingEnvironment.WebRootPath}\\Report\\ChequePrint\\ChequePrint.rdlc";

                Dictionary<string, string> para = new Dictionary<string, string>();
                para.Add("prm", "RDLC Report");

                LocalReport rpt = new LocalReport(reportRdlcPath);
                rpt.AddDataSource("dsChequePrint", checkPrintLetterDetail);

                var reportResultLetter = rpt.Execute(RenderType.Pdf, extensionCheckPrint, para, mimetypeCheckPrint);

                var safeName = string.Join("_", model.ChequeName.Split(Path.GetInvalidFileNameChars()));
                var dateForFileName = model.Date.ToString("yyyy_MM_dd");
                var fileName = $"Cheque_{safeName}_{dateForFileName}.pdf";

                return (reportResultLetter.MainStream, fileName);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error generating cheque print: {ex.Message}", ex);
            }
        }

        public async Task<(byte[] Content, string FileName)> ChequePrintAttachmentUploadAsync(ChequePrintAttachmentUploadDTO model)
        {
            try
            {
                var transactionOptions = new TransactionOptions { Timeout = TimeSpan.FromMinutes(5), IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted };
                using (var transactionScope = new TransactionScope(TransactionScopeOption.Required, transactionOptions, TransactionScopeAsyncFlowOption.Enabled))
                {
                    if (model.Files.Count == 0)
                    {
                        throw new Exception("No file uploaded");
                    }

                    using var zipMemoryStream = new MemoryStream();
                    using (var zipArchive = new ZipArchive(zipMemoryStream, ZipArchiveMode.Create, leaveOpen: true))
                    {
                        foreach (var file in model.Files)
                        {
                            if (file == null) continue;

                            var basePathInProj = Path.Combine(_hostingEnvironment.WebRootPath, "CheckPrintUpload");
                            if (!Directory.Exists(basePathInProj)) Directory.CreateDirectory(basePathInProj);

                            var fileName = Path.GetFileName(file.FileName);
                            var extension = Path.GetExtension(file.FileName);

                            if (extension != ".xlsx")
                            {
                                throw new Exception("Invalid Template");
                            }

                            var filePath = Path.Combine(basePathInProj, $"{fileName}{extension}");

                            if (!File.Exists(filePath))
                            {
                                using (var stream = new FileStream(filePath, FileMode.Create))
                                {
                                    await file.CopyToAsync(stream);
                                }
                            }

                            // Read excel
                            using var wbook = new XLWorkbook(filePath);
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
                            var lastRow = lastRowUsed?.RowNumber();

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

                                _checkPrintDataList.Add(new CheckPrintDataDTO
                                {
                                    EmployeeName = _employeeName,
                                    Date = _date,
                                    Amount = _amount
                                });
                            }

                            var usedFileNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                            foreach (var item in _checkPrintDataList)
                            {
                                if (!DateTime.TryParse(item.Date, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate))
                                {
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

                                var cleanAmount = item.Amount.Replace(",", "").Trim();
                                if (!decimal.TryParse(cleanAmount, NumberStyles.Number, CultureInfo.InvariantCulture, out var amountDecimal))
                                {
                                    throw new Exception($"Invalid amount for employee '{item.EmployeeName}': '{item.Amount}'");
                                }

                                string Amount = amountDecimal.ToString("N2", CultureInfo.InvariantCulture);
                                string _amountInWord = ConvertAmountToWords(amountDecimal);

                                string mimetypeCheckPrint = "";
                                int extensionCheckPrint = 1;

                                var checkPrintLetterDetail = new List<CheckPrintDataSetDTO> {
                                    new CheckPrintDataSetDTO {
                                        EmployeeName = $"{item.EmployeeName}",
                                        Amount = $"{Amount}",
                                        Year1 = $"{Year1}", Year2 = $"{Year2}", Year3 = $"{Year3}", Year4 = $"{Year4}",
                                        Month1 = $"{Month1}", Month2 = $"{Month2}",
                                        Date1 = $"{Date1}", Date2 = $"{Date2}",
                                        AmountInWord = _amountInWord
                                    }
                                };

                                var reportRdlcPath = $"{_hostingEnvironment.WebRootPath}\\Report\\ChequePrint\\ChequePrint.rdlc";

                                Dictionary<string, string> para = new Dictionary<string, string>();
                                para.Add("prm", "RDLC Report");

                                LocalReport rpt = new LocalReport(reportRdlcPath);
                                rpt.AddDataSource("dsChequePrint", checkPrintLetterDetail);

                                var reportResultLetter = rpt.Execute(RenderType.Pdf, extensionCheckPrint, para, mimetypeCheckPrint);

                                var safeEmployeeName = string.Join("_", item.EmployeeName.Split(Path.GetInvalidFileNameChars()));
                                var dateForFileName = parsedDate.ToString("yyyy_MM_dd");
                                var uniqueTimestamp = DateTime.Now.Ticks.ToString().Substring(DateTime.Now.Ticks.ToString().Length - 6);

                                var pdfEntryName = $"{safeEmployeeName}_{dateForFileName}_{uniqueTimestamp}.pdf";

                                var counter = 1;
                                var uniqueEntryName = pdfEntryName;
                                while (!usedFileNames.Add(uniqueEntryName))
                                {
                                    uniqueEntryName = $"{safeEmployeeName}_{dateForFileName}_{uniqueTimestamp}_{counter}.pdf";
                                    counter++;
                                }

                                var zipEntry = zipArchive.CreateEntry(uniqueEntryName, CompressionLevel.Optimal);
                                using (var entryStream = zipEntry.Open())
                                {
                                    await entryStream.WriteAsync(reportResultLetter.MainStream, 0, reportResultLetter.MainStream.Length);
                                }
                            }

                            if (File.Exists(filePath))
                            {
                                File.Delete(filePath);
                            }
                        }
                    }

                    transactionScope.Complete();

                    var zipFileName = $"Cheque_Print_{DateTime.Now:yyyyMMdd_HHmmss}.zip";
                    return (zipMemoryStream.ToArray(), zipFileName);
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
        private static readonly string[] _scale = { "", "Thousand", "Million", "Billion", "Trillion" };

        private static string ConvertAmountToWords(decimal amount)
        {
            var wholePart = (long)Math.Floor(amount);
            var fractionalPart = (int)Math.Round((amount - wholePart) * 100);

            var words = new StringBuilder();

            if (wholePart > 0)
            {
                words.Append(ConvertWholeNumberToWords(wholePart));
                words.Append(" Rupees");
            }
            else
            {
                words.Append("Zero Rupees");
            }

            if (fractionalPart > 0)
            {
                if (wholePart > 0)
                {
                    words.Append(" and ");
                }
                words.Append(ConvertFractionToWords(fractionalPart));
                words.Append(" Cents");
            }

            words.Append(" Only");

            return words.ToString().Trim();
        }

        private static string ConvertWholeNumberToWords(long number)
        {
            if (number == 0) return "Zero";

            if (number < 0)
            {
                return "Negative " + ConvertWholeNumberToWords(Math.Abs(number));
            }

            var words = new StringBuilder();
            int groupIndex = 0;

            while (number > 0)
            {
                var group = (int)(number % 1000);
                number /= 1000;

                if (group > 0)
                {
                    var groupWords = ConvertGroupToWords(group);
                    var scale = _scale[groupIndex];

                    if (!string.IsNullOrEmpty(scale))
                    {
                        groupWords += " " + scale;
                    }

                    if (words.Length > 0)
                    {
                        if (number == 0 && group < 100)
                        {
                            words.Insert(0, groupWords + " ");
                        }
                        else
                        {
                            words.Insert(0, groupWords + " ");
                        }
                    }
                    else
                    {
                        words.Append(groupWords);
                    }
                }

                groupIndex++;
            }

            return words.ToString().Trim();
        }

        private static string ConvertGroupToWords(int number)
        {
            if (number == 0) return "";

            var words = new StringBuilder();

            var hundreds = number / 100;
            var remainder = number % 100;

            if (hundreds > 0)
            {
                words.Append(_units[hundreds] + " Hundred");
                if (remainder > 0)
                {
                    words.Append(" ");
                }
            }

            if (remainder > 0)
            {
                if (remainder < 20)
                {
                    words.Append(_units[remainder]);
                }
                else
                {
                    words.Append(_tens[remainder / 10]);
                    if (remainder % 10 > 0)
                    {
                        words.Append(" " + _units[remainder % 10]);
                    }
                }
            }

            return words.ToString().Trim();
        }

        private static string ConvertFractionToWords(int cents)
        {
            if (cents == 0) return "";

            if (cents < 20)
            {
                return _units[cents];
            }
            else
            {
                var tens = _tens[cents / 10];
                var remainder = cents % 10;
                if (remainder > 0)
                {
                    return tens + " " + _units[remainder];
                }
                return tens;
            }
        }
    }
}