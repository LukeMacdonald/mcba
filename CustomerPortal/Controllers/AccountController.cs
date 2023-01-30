using CustomerPortal.Data;
using CustomerPortal.Models;
using CustomerPortal.Models.Enum;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace CustomerPortal.Controllers;

[AuthoriseCustomer]
public class AccountController : Controller
{
    
    private const string SessionKey_Account = "AccountController_Account";
    private static decimal ATMServiceFee = 0.05M;
    private static decimal TransferServiceFee = 0.10M;
    
    private readonly MCBAContext _context;
    
    
    // Loads the CustomerID entered at Login
    private int CustomerID => HttpContext.Session.GetInt32(nameof(Customer.CustomerID)).Value;
   

    public AccountController(MCBAContext context)
    {
        _context = context;
    }

    public IActionResult Deposit(int accountNumber = 0)
    {
        // Sets Transaction Type to Deposit
        var type = TransactionType.Deposit;
        // Redirects to Transaction Action
        return RedirectToAction(nameof(Transaction), new { accountNumber, type });
    }
    
    public IActionResult Withdraw(int accountNumber = 0)
    {
        // Sets Transaction Type to Withdraw
        var type = TransactionType.Withdraw;
        // Redirects to Transaction Action
        return RedirectToAction(nameof(Transaction), new { accountNumber, type });
    }
    
    public IActionResult Transfer(int accountNumber = 0)
    {
        // Sets Transaction Type to Transfer
        var type = TransactionType.Transfer;
        // Redirects to Transaction Action
        return RedirectToAction(nameof(Transaction), new { accountNumber, type });
    }
    
    public async Task<IActionResult> Transaction(TransactionType type,int accountNumber = 0)
    {

        var customer = await FindCustomer(CustomerID);
        
        // Sets default balance
        decimal balance = 0;

        // Sets default accountType
        AccountType accountType = AccountType.Saving;

        // sets default Transaction/Withdraw count
        int count = 0;
        
        // Checks account number is not set to 0
        if (accountNumber != 0)
        {
            // Gets account object using accountNumber selected by user
            var account = await FindAccount(accountNumber);
            
            // If account is null redirects user to error page
            if (account == null)
            {
                return RedirectToAction("Error","Home");
            }
            
            // sets balance for viewModel
            balance = account.Balance;
            
            // sets accountType for viewModel
            accountType = account.AccountType;

            count = await TotalPayments(accountNumber);
            
        }
        // Returns TransactionViewModel to View
        return View(new TransactionViewModel() 
            { 
                TransactionType = type, 
                AccountNumber = accountNumber, 
                CustomerAccounts = customer.Accounts, 
                AccountBalance = balance, 
                AccountType = accountType,
                TotalPayments = count,
            }
        );
    }
    
    [HttpPost]
    public async Task<IActionResult> Transaction(TransactionViewModel viewModel)
    {
        if (viewModel.TransactionType == TransactionType.Transfer)
        {
            // Gets the account that the DestinationAccountNumber refers to
            var destinationAccount = await FindAccount(viewModel.DestinationAccountNumber);
            
            // If DestinationAccount is null than returns to view with error
            if (destinationAccount == null)
            {
                ModelState.AddModelError(nameof(viewModel.DestinationAccountNumber),"Account Doesnt Exist!");
                var customer = await FindCustomer(CustomerID);
                viewModel.CustomerAccounts= customer.Accounts;
                return View(viewModel);
            }
        }
        if (!ModelState.IsValid)
        {
            var customer = await FindCustomer(CustomerID);
            viewModel.CustomerAccounts = customer.Accounts;
            return View(viewModel);
        }
        
        // Stores the TransactionViewModel into the Session
        HttpContext.Session.Set(nameof(viewModel),viewModel);
        
        // Redirects to Confirmation Page
        return RedirectToAction(nameof(ConfirmationPage),viewModel);
    }
    
    public IActionResult ConfirmationPage(TransactionViewModel viewModel)
    {
        return View(viewModel);
    }
    
    public async Task<IActionResult> CompleteTransaction()
    {
        // Gets TransactionViewModel from Session
        var viewModel = HttpContext.Session.Get<TransactionViewModel>("viewModel");
        
        // Finds account using AccountNumber Stored in Model
        var account = await FindAccount(viewModel.AccountNumber, CustomerID);
        
        
        if (account == null) { return RedirectToAction("Error","Home"); }
        
        //Performs Actions depending on TransactionType
        switch (viewModel.TransactionType)
        {
            // If Deposit than adds the amount entered to users account
            case TransactionType.Deposit:
                account.Balance += viewModel.PaymentAmount;
                break;
            // If Withdraw than subtracts the amount from users account
            case TransactionType.Withdraw:
                
                if (viewModel.TotalPayments >= 2)
                {
                    await ServiceFee(viewModel);
                }
                
                account.Balance -= viewModel.PaymentAmount;
                
                break;
            
            // If transfer than subtracts the amount from users account and sets account
            // to the account that the desiredAccountNumber points to.
            case TransactionType.Transfer:
                
                if (viewModel.TotalPayments >= 2)
                {
                    await ServiceFee(viewModel);
                }
                
                account.Balance -= viewModel.PaymentAmount;

                account.Transactions.Add(
                        new Transaction
                        {
                            TransactionType = viewModel.TransactionType,
                            Amount = viewModel.PaymentAmount,
                            TransactionTimeUtc = DateTime.UtcNow,
                            Comment = viewModel.Comment,
                            DestinationAccountNumber = viewModel.DestinationAccountNumber
                        });
                
                await _context.SaveChangesAsync();
                
                account = await FindAccount(viewModel.DestinationAccountNumber);
                
                if (account == null)
                {
                    return RedirectToAction("Error","Home");
                }
                account.Balance += viewModel.PaymentAmount;
                break;
        }
        
        // Creates the new transaction
        account.Transactions.Add(
            new Transaction
            {
                TransactionType = viewModel.TransactionType,
                Amount = viewModel.PaymentAmount,
                TransactionTimeUtc = DateTime.UtcNow,
                Comment = viewModel.Comment,
            });
        
        // Saves changes to Database
        await _context.SaveChangesAsync();
        
        // Returns user to Home
        return RedirectToAction("Index", "Customer", new { area = "" });
    }
    
    public IActionResult IndexToViewStatements(int accountNumber = 0)
    {
        HttpContext.Session.SetInt32(SessionKey_Account,accountNumber);
        return RedirectToAction(nameof(ViewStatements));
    }
    
    public async Task<IActionResult> ViewStatements(int? page = 1)
    {
        var accountNumber = HttpContext.Session.GetInt32(SessionKey_Account).Value;

        var account = await FindAccount(accountNumber, CustomerID);
        
        if (account == null && accountNumber != 0)
        {
            return RedirectToAction("Index", "Customer", new { area = "" }); // OR return BadRequest();
        }
        
        var customer = await FindCustomer(CustomerID);
        
        ViewBag.Account = accountNumber;
        ViewBag.Accounts = customer.Accounts.ToList();

        // Page the orders, maximum of 3 per page.
        const int pageSize = 4;
        var pagedList = await _context.Transaction.Where(x => x.AccountNumber == accountNumber).
            OrderByDescending(x => x.TransactionTimeUtc).ToPagedListAsync(page, pageSize);

        return View(pagedList);
    }

    // Method for checking that the amount of money being removed from the account is valid
    public bool IsValid(Account account, decimal amount)
    {
        if ((account.AccountType == AccountType.Saving && account.Balance - amount < 0) ||
            (account.AccountType == AccountType.Checking && account.Balance - amount < 300))
        {
            return false;

        }
        return true;
    }

    public async Task ServiceFee(TransactionViewModel viewModel)
    {
        
            var account = await _context.Account.FindAsync(viewModel.AccountNumber);
            
            if (account == null) { return;}
                                                      
            decimal amount;
                string comment;
                if (viewModel.TransactionType == TransactionType.Transfer)
                {
                    amount = TransferServiceFee;
                    comment = "Transfer Service Fee";
                }
                else
                {
                    amount = ATMServiceFee;
                    comment = "ATM Service Fee";
                }
                
                account.Balance -= amount;
                
                await _context.Transaction.AddAsync(new Transaction()
                {
                    AccountNumber = viewModel.AccountNumber,
                    TransactionTimeUtc = DateTime.UtcNow,
                    TransactionType = TransactionType.ServiceCharge,
                    Amount = amount,
                    Comment = comment,
                });
                
                await _context.SaveChangesAsync();
                
    }
    
    // Method for Getting the total number of Tranfers and Withdraws an Account
    // Has performed
    public async Task<int> TotalPayments(int accountNumber)
    {
        return await _context.Transaction.CountAsync(x =>
            x.AccountNumber == accountNumber && (x.TransactionType == TransactionType.Transfer ||
                                                 x.TransactionType == TransactionType.Withdraw));
    }

    // Method for Finding Account with the Primary Key AccountNumber
    public async Task<Account> FindAccount(int accountNumber)
    {
        return await _context.Account.Include(x => x.Transactions).Include(x => x.BillPay)
            .FirstOrDefaultAsync(x => x.AccountNumber == accountNumber);
    }
    // Method for Finding Account with the Primary Key AccountNumber and ForeignKey CustomerID
    public async Task<Account> FindAccount(int accountNumber,int customerID)
    {
        return await _context.Account.Include(x => x.Transactions).Include(x => x.BillPay)
            .FirstOrDefaultAsync(x => x.AccountNumber == accountNumber && x.CustomerID == customerID);
    }
    // Method for Finding Customer with the Primary Key CustomerID
    public async Task<Customer> FindCustomer(int customerID)
    {
        return await _context.Customer.Include(x => x.Accounts).Include(x=>x.Login)
            .FirstOrDefaultAsync(x => x.CustomerID == customerID);
    }
}