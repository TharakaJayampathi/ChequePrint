namespace ChequePrint.ViewModels
{
    public class ChequePrintReportViewModel
    {
        public string ChequeName { get; set; }
        public DateTime Date { get; set; }
        public double Amount { get; set; }
        public DateTime PrintedOn { get; set; }
    }
}