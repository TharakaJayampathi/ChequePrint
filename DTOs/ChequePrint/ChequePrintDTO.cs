namespace ChequePrint.DTOs.ChequePrint
{
    public class ChequePrintDTO
    {
        public byte PaymentMethod { get; set; }
        public string ChequeName { get; set; }
        public DateTime Date { get; set; }
        public double Amount { get; set; }
    }
}