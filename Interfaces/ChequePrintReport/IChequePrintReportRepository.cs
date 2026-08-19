using ChequePrint.DTOs.ChequePrint;

namespace ChequePrint.Interfaces.ChequePrintReport
{
    public interface IChequePrintReportRepository
    {
        Task<(byte[] Content, string FileName)> ChequePrintAsync(CheckPrintDTO model);
        Task<(byte[] Content, string FileName)> ChequePrintAttachmentUploadAsync(ChequePrintAttachmentUploadDTO model);
    }
}