using CustomerPortal.Data;
using CustomerPortal.Models;
using CustomerPortal.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace CustomerPortal.Controllers;

public class BillPayController : Controller
{
    private readonly MCBAContext _context;
    
    // Loads the CustomerID entered at Login
    private int CustomerID => HttpContext.Session.GetInt32(nameof(Customer.CustomerID)).Value;

    public BillPayController(MCBAContext context)
    {
        _context = context;
    }
    public async Task<IActionResult> CreateBill(int accountNumber)
    {

        var account = await FindAccount(accountNumber);
        
        if (account == null) { return RedirectToAction("Error","Home"); }
        
        var retDateTime = DateTime.Now;
        
        // Sets datetime to the current day and time and rounds the seconds to 0
        var dateTime = new DateTime(retDateTime.Year, retDateTime.Month,
            retDateTime.Day, retDateTime.Hour, retDateTime.Minute, 0);

        // gets list of all payees available
        var payees =  _context.Payee.ToList();
        
        // returns BillViewModel to View
        return View(
            new BillViewModel()
            {
                ScheduledTime = dateTime,
                AccountNumber = accountNumber,
                Balance = account.Balance,
                AccountType = account.AccountType,
                Payees = payees

            });
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateBill(BillViewModel viewModel)
    {
        // Creates new BillPay to database using the data from BillViewModel
        _context.BillPay.Add(new BillPay()
        {
            Active = true,
            AccountNumber = viewModel.AccountNumber,
            ScheduleTimeUtc = viewModel.ScheduledTime.ToUniversalTime(),
            PayID = viewModel.BillPay.PayID,
            Amount = viewModel.BillPay.Amount,
            Period = viewModel.BillPay.Period
        });
        // Saves changes to Database
        await _context.SaveChangesAsync();
        return RedirectToAction("ViewBill", new{accountNumber = viewModel.AccountNumber});
    }
    
    public IActionResult ViewBill(int accountNumber)
    {
        
        HttpContext.Session.SetInt32(nameof(ViewBills),accountNumber);
        return RedirectToAction(nameof(ViewBills));
        
    }
    
    public async Task<IActionResult> ViewBills(int? page = 1)
    {
        var accountNumber = HttpContext.Session.GetInt32(nameof(ViewBills));

        var customer = await FindCustomer(CustomerID);
        
        ViewBag.Accounts = customer.Accounts.ToList();

        var account = await FindAccount(accountNumber.Value, CustomerID);

        if (account == null && accountNumber != 0)
        {
            return RedirectToAction("Index", "Customer");
        }
        
        ViewBag.Account = account;
        
        ViewBag.AccountNumber = accountNumber;

        // Page the orders, maximum of 5 per page.
        const int pageSize = 5;
        var pagedList = await _context.BillPay.Include(x=>x.Payee).Where(x => x.AccountNumber == accountNumber && !x.Failed).
            OrderBy(x => x.ScheduleTimeUtc).ToPagedListAsync(page, pageSize);
        
        return View(pagedList);
    }
    
    public async Task<IActionResult> CancelBill(int billID)
    {
        var bill = await FindBillPay(billID);

        if (bill != null)
        {
            _context.BillPay.Remove(bill);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ViewBills));
        }
        else
        {
            return RedirectToAction("Error","Home");
        }
       
        
    }
    
    // Method process the payment that the BillPay refers to.
    // Creates a new Transaction entry in the Database with 
    // the details from the billpay
    public async Task<IActionResult> PayBill(int billID)
    {
        
        var bill = await FindBillPay(billID);
        
        if (bill == null) { return RedirectToAction("Error", "Home");}
        
        var account = await FindAccount(bill.AccountNumber);

        if (account == null) { return RedirectToAction("Error", "Home");}

        decimal balance = account.Balance - bill.Amount;

        // Checks that the transaction payment will be valid for the accountType
        if ((account.AccountType == AccountType.Checking && balance < 300) ||
            (account.AccountType == AccountType.Saving && balance < 0))
        {
            // If the payment is invalid than the failed column in the object
            // is set to try and the bill entry in the database is updated
            bill.Failed = true;
            _context.BillPay.Update(bill);
        }
        else
        {
            bill.Failed = false;
        }
        // If bill is disabled than doesnt allow payment
        if (!bill.Active || bill.Failed)
        {
            return RedirectToAction(nameof(UnpaidBills));
        }
        else if (!bill.Active)
        {
            return RedirectToAction("Error","Home");
        }
        else
        {
            // New Transaction Created From BillPay data
            account.Balance -= bill.Amount;
            var transaction = new Transaction()
            {
                AccountNumber = bill.AccountNumber,
                Amount = bill.Amount,
                TransactionType = TransactionType.BillPay,
                TransactionTimeUtc = bill.ScheduleTimeUtc,
                Comment = $"{bill.Payee.Name}"
                
            };
            // If the billpay is montly than a new billpay is entered for the same
            // details but the date is set for the next month.
            if (bill.Period == PeriodType.Monthly)
            {
                await _context.BillPay.AddAsync(new BillPay()
                { 
                    AccountNumber = bill.AccountNumber,
                    Active = bill.Active,
                    Amount = bill.Amount,
                    PayID = bill.PayID,
                    ScheduleTimeUtc = bill.ScheduleTimeUtc.AddMonths(1),
                    Period = bill.Period,
                    Failed = false
                
                });
            }
            // Removes succesfully paid BillPay from database
            _context.BillPay.Remove(bill);
            await _context.Transaction.AddAsync(transaction);
        }
        
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(ViewBills));
        
    }

    // Method to begin the process of updating bill information and rif valid returns the 
    // user to the view that allows them to enter the new data.
    public async Task<IActionResult> EditBill(int billID)
    {
        
        var bill = await FindBillPay(billID);
        
        if (bill == null) { return RedirectToAction("Error", "Home");}

        var customer = await FindCustomer(CustomerID);

        bool valid = true;
        
        foreach (var account in customer.Accounts)
        {
            if (bill.AccountNumber == account.AccountNumber)
            {
                valid = true;
                break;
            }
            valid = false;
        }

        if (valid)
        {
            return View(
                new BillViewModel()
                {
                    BillPayID = billID,
                    ScheduledTime = bill.ScheduleTimeUtc.ToLocalTime(),
                    Amount = bill.Amount,
                    Accounts = customer.Accounts,
                    AccountNumber = bill.AccountNumber
                });
        }
        return RedirectToAction(nameof(ViewBills));
    }
    
    // Method for updating the bill information in the BillPay Table in database
    [HttpPost]
    public async Task<IActionResult> EditBill(BillViewModel viewModel)
    {
        
        var bill = await FindBillPay(viewModel.BillPayID);

        if (bill != null)
        {
            bill.Failed = false;
            bill.ScheduleTimeUtc = viewModel.ScheduledTime.ToUniversalTime();
            bill.Amount = viewModel.Amount;
            bill.AccountNumber = viewModel.AccountNumber;
        
            _context.BillPay.Update(bill);
            await _context.SaveChangesAsync();
        }
        
       
        return RedirectToAction(nameof(ViewBills));

    }
    
    public async Task<IActionResult> UnpaidBills(int? page = 1)
    {
        var accountNumber = HttpContext.Session.GetInt32(nameof(ViewBills));
        if (accountNumber == null)
        {
            return RedirectToAction("Error", "Home");
        }
        
        var account = await FindAccount(accountNumber.Value);
        
        ViewBag.Account = account;

        // Page the orders, maximum of 3 per page.
        const int pageSize = 5;
        
        var pagedList = await _context.BillPay.Include(x=>x.Payee).Where(x => x.AccountNumber == account.AccountNumber && x.Failed).
            OrderBy(x => x.ScheduleTimeUtc).ToPagedListAsync(page, pageSize);
       
        return View(pagedList);
    }

    public async Task<BillPay> FindBillPay(int billID)
    {
        return await _context.BillPay.FindAsync(billID);
    }
    public async Task<Account> FindAccount(int accountNumber)
    {
        return await _context.Account.Include(x => x.Transactions).Include(x => x.BillPay)
            .FirstOrDefaultAsync(x => x.AccountNumber == accountNumber);
    }
    public async Task<Account> FindAccount(int accountNumber,int customerID)
    {
        return await _context.Account.Include(x => x.Transactions).Include(x => x.BillPay)
            .FirstOrDefaultAsync(x => x.AccountNumber == accountNumber && x.CustomerID == customerID);
    }
    public async Task<Customer> FindCustomer(int customerID)
    {
        return await _context.Customer.Include(x => x.Accounts).Include(x=>x.Login)
            .FirstOrDefaultAsync(x => x.CustomerID == customerID);
    }

}