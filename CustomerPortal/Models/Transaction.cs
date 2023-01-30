using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using JetBrains.Annotations;
using CustomerPortal.Models.Enum;
using CustomerPortal.Models.Validation;


namespace CustomerPortal.Models;

public class Transaction
{
    // Primary Key of Transaction
    public int TransactionID { get; set; }
    
    public TransactionType TransactionType { get; set; }
    
    [ForeignKey("Account")]
    public int AccountNumber { get; set; }
    public virtual Account Account { get; set; }
    
    [AccountNumberLength]
    [ForeignKey("DestinationAccount")]
    public int? DestinationAccountNumber { get; set; }
    public virtual Account DestinationAccount { get; set; }

    [Column(TypeName = "money"),TwoDecimalPlaces]
    public decimal Amount { get; set; }

    [StringLength(30)]
    [CanBeNull]
    public string Comment { get; set; }

    public DateTime TransactionTimeUtc { get; set; }

}