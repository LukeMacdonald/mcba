using CustomerPortal.Models.Enum;

namespace CustomerPortal.Models.ViewModels;

public class BillViewModel
{
    public int BillPayID { get; set; }
    public int AccountNumber { get; set; }
    
    public AccountType AccountType { get; set; }

    public int PayID;
    public BillPay BillPay { get; set; }
    
    public decimal Balance { get; set; }
    
    public DateTime ScheduledTime { get; set; }
    
    public Account Account { get; set; }
    
    public decimal Amount { get; set; }
    
    public ICollection<Account> Accounts { get; set; }
    
    public ICollection<Payee> Payees { get; set; }
    
    public PeriodType PeriodType { get; set; }
    
    public string PayeeName { get; set; }
    
    public override bool Equals(Object obj)
    {
        
        BillViewModel y = (BillViewModel)obj;
        bool valid = Accounts.All(y.Accounts.Contains) && y.Accounts.All(Accounts.Contains);

        return AccountNumber == y.AccountNumber && BillPayID == y.BillPayID &&
               ScheduledTime == y.ScheduledTime && Amount == y.Amount && valid;
    }
}