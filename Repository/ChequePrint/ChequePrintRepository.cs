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

                                var TempHeader = "HRIS NoEmployee NameDivision";
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

                                for (var i = 2; i <= lastRow; i++)
                                {
                                    var _employeeName = ws.Cell($"A{i}").GetValue<string>().Trim();
                                    var _amountDecimal = Convert.ToDecimal(0.0);
                                    var _amount = ws.Cell($"B{i}").GetValue<string>();
                                    if (!string.IsNullOrEmpty(_amount))
                                    {
                                        _amountDecimal = Convert.ToDecimal(ws.Cell($"B{i}").GetValue<string>());
                                    }
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