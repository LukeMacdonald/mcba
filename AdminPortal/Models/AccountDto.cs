using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Transactions;
using AdminPortal.Enum;

namespace AdminPortal.Models;

public class AccountDto
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
    [Display(Name = "Account Number")]
    public int AccountNumber { get; set; }

    [Display(Name = "Type")] public AccountType AccountType { get; set; }

    // Foreign Key & Navigation Property for Customer Table
    public int CustomerID { get; set; }
    public virtual CustomerDto Customer { get; set; }

    [Column(TypeName = "money")]
    [DataType(DataType.Currency)] //,TwoDecimalPlaces,IsPositive]
    public decimal Balance { get; set; }

    // Set ambiguous navigation property with InverseProperty annotation or Fluent-API in the McbaContext.cs file.
    [InverseProperty(nameof(TransactionDto.Account))]
    public virtual ICollection<Transaction> Transactions { get; } = new List<Transaction>();

    [InverseProperty(nameof(AccountDto))] 
    public virtual ICollection<BillPayDto> BillPay { get; } = new List<BillPayDto>();
}