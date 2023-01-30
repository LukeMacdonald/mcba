using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace CustomerPortal.Models;

public class Account 
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
    [Display(Name = "Account Number")]
    public int AccountNumber { get; set; }
    
    [Display(Name = "Type")]
    public AccountType AccountType { get; set; }
    
    // Foreign Key & Navigation Property for Customer Table
    public int CustomerID { get; set; }
    public virtual Customer Customer { get; set; }
    
    [Column(TypeName = "money")]
    // [DataType(DataType.Currency),TwoDecimalPlaces,IsPositive]
    public decimal Balance { get; set; }

    // Set ambiguous navigation property with InverseProperty annotation or Fluent-API in the McbaContext.cs file.
    [InverseProperty(nameof(Transaction.Account))]
    public virtual ICollection<Transaction> Transactions { get; } = new List<Transaction>();
    
    // public virtual List<BillPay> BillPays { get; set; }
    [InverseProperty(nameof(Account))]
    public virtual ICollection<BillPay> BillPay { get; } = new List<BillPay>();

    public override string ToString()
    {
        return AccountNumber.ToString() + " (" + Enum.GetName(AccountType) + ")";
    }
    public override bool Equals(object obj)
    {
        Account y = (Account)obj;
        return AccountNumber == y.AccountNumber && Balance == y.Balance && AccountType == y.AccountType &&
               CustomerID == y.CustomerID;
    }
}