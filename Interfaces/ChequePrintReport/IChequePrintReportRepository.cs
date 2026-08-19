using ChequePrint.ViewModels;

namespace ChequePrint.Interfaces.ChequePrintReport
{
    public interface IChequePrintReportRepository
    {
        Task<List<ChequePrintReportViewModel>> GetAllAsync();
    }
}