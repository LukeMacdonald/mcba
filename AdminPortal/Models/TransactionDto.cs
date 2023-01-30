using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AdminPortal.Enum;

namespace AdminPortal.Models;

public class TransactionDto
{
    // Primary Key of Transaction
    public int TransactionID { get; set; }
    
    public TransactionType TransactionType { get; set; }
    
    [ForeignKey("Account")]
    public int AccountNumber { get; set; }
    public virtual AccountDto Account { get; set; }
    
    //[AccountNumberLength]
    [ForeignKey("DestinationAccount")]
    public int? DestinationAccountNumber { get; set; }
    public virtual AccountDto DestinationAccount { get; set; }

    [Column(TypeName = "money")]//,TwoDecimalPlaces]
    public decimal Amount { get; set; }

    [StringLength(30)]
    public string Comment { get; set; }

    public DateTime TransactionTimeUtc { get; set; }
}