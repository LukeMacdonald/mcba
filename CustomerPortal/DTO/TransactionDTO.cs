namespace CustomerPortal.DTO;

public class TransactionDTO
{
    public int TransactionID { get; set; }
    public string TransactionType { get; set; }
    public int AccountNumber { get; set; }
    public int? DestinationAccountNumber { get; set; }
    public decimal Amount { get; set; }
    public string Comment { get; set; }
    public DateTime TransactionTimeUTC { get; set; }
}