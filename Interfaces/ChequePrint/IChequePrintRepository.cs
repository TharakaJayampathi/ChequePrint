using ChequePrint.DTOs.ChequePrint;

namespace ChequePrint.Interfaces.ChequePrint
{
    public interface IChequePrintRepository
    {
        Task<(byte[] Content, string FileName)> ChequePrintAsync(ChequePrintDTO model);
        Task<(byte[] Content, string FileName)> ChequePrintAttachmentUploadAsync(ChequePrintAttachmentUploadDTO model);
    }
}