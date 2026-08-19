using ChequePrint.Interfaces.ChequePrintReport;
using ChequePrint.ViewModels;

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
                return _checkPrintTrackings;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}