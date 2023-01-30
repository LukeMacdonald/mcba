using CustomerPortal.Data;
using CustomerPortal.Models;
using Microsoft.EntityFrameworkCore;

namespace CustomerPortal.Services;

public class BillPayService : BackgroundService
{
    
    private readonly IServiceProvider _services;
    
    public BillPayService(IServiceProvider services)
    {
        _services = services;
    }
    
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        
        while(!cancellationToken.IsCancellationRequested)
        {
            await DoWork(cancellationToken);
            await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken);
        }
    }

    private async Task DoWork(CancellationToken cancellationToken)
    { 
        using var scope = _services.CreateScope();
        
        var context = scope.ServiceProvider.GetRequiredService<MCBAContext>();
        
        var retDateTime = DateTime.UtcNow;
        
        var dt = new DateTime(retDateTime.Year, retDateTime.Month,
                retDateTime.Day, retDateTime.Hour, retDateTime.Minute, 0);
        
        var  all = await context.BillPay.ToListAsync(cancellationToken);

        var overdue = all.Where(x => x.ScheduleTimeUtc < dt);
        
        var due = all.Where(x => x.ScheduleTimeUtc == dt);
        
        foreach (var bill in overdue)
        {
            await PayBill(bill,cancellationToken);
        }
            
        foreach (var bill in due)
        {
            await PayBill(bill,cancellationToken);
        }
    }

    private async Task PayBill(BillPay bill,CancellationToken cancellationToken)
    {
        using var scope = _services.CreateScope();
        
        var context = scope.ServiceProvider.GetRequiredService<MCBAContext>();
        
        var account = await context.Account.FindAsync(bill.AccountNumber);
        
        if (account == null) { return;}
        
        var payee = await context.Payee.FindAsync(bill.PayID);
        
        if (payee == null) { return;}

        decimal balance = account.Balance - bill.Amount;

        if ((account.AccountType == AccountType.Checking && balance < 300) ||
            (account.AccountType == AccountType.Saving && balance < 0))
        {
            bill.Failed = true;
            context.BillPay.Update(bill);
        }
        else if (!bill.Active || bill.Failed)
        {
            
        }
        else
        {
            account.Balance -= bill.Amount;
            var transaction = new Transaction()
            {
                AccountNumber = bill.AccountNumber,
                Amount = bill.Amount,
                TransactionType = TransactionType.BillPay,
                TransactionTimeUtc = bill.ScheduleTimeUtc,
                Comment = $"{payee.Name}"
                
            };
            if (bill.Period == PeriodType.Monthly)
            {
                await context.BillPay.AddAsync(new BillPay()
                { 
                    AccountNumber = bill.AccountNumber,
                    Active = bill.Active,
                    Amount = bill.Amount,
                    PayID = bill.PayID,
                    ScheduleTimeUtc = bill.ScheduleTimeUtc.AddMonths(1),
                    Period = bill.Period,
                    Failed = false
                
                },cancellationToken);
            }
            context.BillPay.Remove(bill);
            await context.Transaction.AddAsync(transaction,cancellationToken);
        }
        await context.SaveChangesAsync(cancellationToken);
    }
}
