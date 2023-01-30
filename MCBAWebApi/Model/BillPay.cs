using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MCBAWebApi.Enum;

namespace MCBAWebApi.Models;

public class BillPay
{
    public int BillPayID { get; set; }
    
    [Required,ForeignKey(nameof(Account))]
    public int AccountNumber { get; set; }
    // public virtual Account Account { get; set; }
    
    [Required,ForeignKey(nameof(Payee))]
    public int PayID { get; set; }
    // public virtual Payee Payee { get; set; }
    
    [Required,Column(TypeName = "money")]
    [DataType(DataType.Currency)]
    public decimal Amount { get; set; }
    
    [Required]
    public DateTime ScheduleTimeUtc { get; set; }
    
    [Required]
    public PeriodType Period { get; set; }
    
    // Variable indicating if the payment is still active
    [Required]
    public bool Active { get; set; }
    
    [Required]
    public bool Failed { get; set; }
}