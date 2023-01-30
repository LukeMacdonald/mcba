namespace MCBAWebApi.Models.DataManager;

using MCBAWebApi.Data;
using MCBAWebApi.Models.Repository;
using MCBAWebApi.Models;

public class AccountManager : IDataRepository<Account, int>
{
    private readonly MCBAContext _context;

    public AccountManager(MCBAContext context)
    {
        _context = context;
    }
    
    // get account from database by id
    public Account Get(int id)
    {
        return _context.Account.Find(id);
    }
    
    // get all accounts from database
    public IEnumerable<Account> GetAll()
    {
        return _context.Account.ToList();
    }

    // add account to the database
    public int Add(Account account)
    {
        _context.Account.Add(account);
        _context.SaveChanges();

        return account.AccountNumber;
    }

    // delete account from database
    public int Delete(int id)
    {
        _context.Account.Remove(_context.Account.Find(id));
        _context.SaveChanges();

        return id;
    }

    // Get accounts which account number contains 
    // specified account number
    public IEnumerable<Account> Search(int accNumber)
    {
        return _context.Account.Where(x => x.AccountNumber.Equals(accNumber));
    } 

    // update account in the database
    public int Update(int id, Account account)
    {
        _context.Update(account);
        _context.SaveChanges();
            
        return id;
    }
}