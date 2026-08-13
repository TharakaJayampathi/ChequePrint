using ChequePrint.DTOs.ChequePrint;

namespace ChequePrint.Interfaces.ChequePrint
{
    public interface IChequePrintRepository
    {
        Task ChequePrintAttachmentUploadAsync(ChequePrintAttachmentUploadDTO model);
    }
}