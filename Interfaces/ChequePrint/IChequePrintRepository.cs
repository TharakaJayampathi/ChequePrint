using ChequePrint.DTOs.ChequePrint;

namespace ChequePrint.Interfaces.ChequePrint
{
    public interface IChequePrintRepository
    {
        Task ChequePrintAsync(CheckPrintDTO model);
        Task<(byte[] Content, string FileName)> ChequePrintAttachmentUploadAsync(ChequePrintAttachmentUploadDTO model);
    }
}