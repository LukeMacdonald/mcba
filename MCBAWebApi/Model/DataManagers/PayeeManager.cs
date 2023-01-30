namespace MCBAWebApi.Models.DataManager;

using MCBAWebApi.Data;
using MCBAWebApi.Models.Repository;
using MCBAWebApi.Models;

public class PayeeManager : IDataRepository<Payee, int>
{
    private readonly MCBAContext _context;
    
    public PayeeManager(MCBAContext context)
    {
        _context = context;
    }
    
    // get payee from the database by id
    public Payee Get(int id)
    {
        return _context.Payee.Find(id);
    }
    
    // get all payees from the database
    public IEnumerable<Payee> GetAll()
    {
        return _context.Payee.ToList();
    }

    // add payee to the database
    public int Add(Payee payee)
    {
        _context.Payee.Add(payee);
        _context.SaveChanges();

        return payee.PayeeID;
    }

    // delete payee from the database
    public int Delete(int id)
    {
        _context.Payee.Remove(_context.Payee.Find(id));
        _context.SaveChanges();

        return id;
    }

    // get payees with payeeID that contains specified int payeeID
    public IEnumerable<Payee> Search(int payeeID)
    {
        return _context.Payee.Where(x => x.PayeeID.Equals(payeeID));
    } 

    // update payee in database
    public int Update(int id, Payee payee)
    {
        _context.Update(payee);
        _context.SaveChanges();
            
        return id;
    }
}