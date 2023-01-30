using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CustomerPortal.Models.Enum;
using CustomerPortal.Models.Validation;

namespace CustomerPortal.Models.ViewModels;

public class TransactionViewModel : IValidatableObject
{
    public int AccountNumber { get; set; }
    
    private static decimal ATMServiceFee = 0.05M;
    
    private static decimal TransferServiceFee = 0.10M;
    
    public AccountType AccountType { get; set; }
    
    public decimal AccountBalance { get; set; }
    
    [Column(TypeName = "money"),TwoDecimalPlaces,IsPositive]
    public decimal PaymentAmount { get; set; }
    
    
    public int DestinationAccountNumber { get; set; }
    
    public TransactionType TransactionType { get; set; }
    
    [StringLength(30)]
    public string Comment { get; set; }
    
    public ICollection<Account> CustomerAccounts { get; set; }
    
    public int TotalPayments { get; set; }
    
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (AccountNumber == DestinationAccountNumber)
        {
            yield return new ValidationResult(
                $"Destination Account Must Be Different!",
                new[] { nameof(DestinationAccountNumber)  });
        }
        
        decimal fee = 0;
        
        if (TransactionType.Equals(TransactionType.Transfer))
        {
            fee = TransferServiceFee;
        }
        else if (TransactionType.Equals(TransactionType.Withdraw))
        {
            fee = ATMServiceFee;
        }
        
        if (TransactionType.Equals(TransactionType.Transfer) || TransactionType.Equals(TransactionType.Withdraw))
        {
            if ((AccountType.Equals(AccountType.Saving) && AccountBalance - PaymentAmount < 0) || (AccountType.Equals(AccountType.Checking) && AccountBalance - PaymentAmount < 300))
            {
                yield return new ValidationResult(
                    $"Insufficient Funds!",
                    new[] { nameof(PaymentAmount)  });
            }
            if (TotalPayments > 2 && ((AccountType.Equals(AccountType.Saving) && AccountBalance - (PaymentAmount + fee) < 0) || (AccountType.Equals(AccountType.Checking) && AccountBalance - (PaymentAmount + fee) < 300) ))
            { 
                yield return new ValidationResult(
                            $"Insufficient Funds for ${fee} Service Fee",
                            new[] { nameof(PaymentAmount) }); 
            }
        }
    }
    public override bool Equals(Object obj)
    {
        
        TransactionViewModel y = (TransactionViewModel)obj;
        bool valid = CustomerAccounts.All(y.CustomerAccounts.Contains) && y.CustomerAccounts.All(CustomerAccounts.Contains);

        return AccountNumber == y.AccountNumber && AccountBalance == y.AccountBalance &&
               DestinationAccountNumber == y.DestinationAccountNumber && PaymentAmount == y.PaymentAmount && valid &&
               Comment == y.Comment && TotalPayments == y.TotalPayments && TransactionType == y.TransactionType;
    }

}