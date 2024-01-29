namespace BankKRT.Presentation.DTOs
{
    public class CustomerTransactionDTO
    {
        public string DocumentSourceAccount { get; set; }
        public string DocumentDestinationAccount { get; set; }
        public int SourceAccount { get; set; }
        public int DestinationAccount { get; set; }
        public decimal TransactionValue { get; set; }
    }
}
 