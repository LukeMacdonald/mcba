namespace CustomerPortal.DTO;

public class AccountDTO
{

    public required int AccountNumber { get; set; }
    public required char AccountType { get; set; }
    public required int CustomerID { get; set; }
    public decimal Amount { get; set; }
    public List<TransactionDTO> Transactions { get; set; }
    
}