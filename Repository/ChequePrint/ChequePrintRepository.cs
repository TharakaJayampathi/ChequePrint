using AspNetCore.Reporting;
using ChequePrint.DTOs.ChequePrint;
using ChequePrint.Interfaces.ChequePrint;
using ClosedXML.Excel;
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
                                    var _amountDecimal = Convert.ToDecimal(0.0);
                                    var _amount = ws.Cell($"C{i}").GetValue<string>();

                                    var _checkPrintData = new CheckPrintDataDTO();
                                    _checkPrintData.EmployeeName = _employeeName;
                                    _checkPrintData.Date = _date;
                                    _checkPrintData.Amount = _amount;

                                    _checkPrintDataList.Add(_checkPrintData);
                                }

                                foreach (var item in _checkPrintDataList)
                                {
                                    string mimetypeCheckPrint = "";
                                    int extensionCheckPrint = 1;

                                    var checkPrintLetterDetail = new List<CheckPrintDataSetDTO> { new CheckPrintDataSetDTO { EmployeeName = $"{item.EmployeeName}", Amount = $"{item.Amount}", Year1 = $"{2}", Year2 = $"{0}", Year3 = $"{2}", Year4 = $"{6}", Month1 = $"{0}", Month2 = $"{8}", Date1 = $"{1}", Date2 = $"{3}", AmountInWord = "Ten Thousand Rupees Only" } };
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
    }
}