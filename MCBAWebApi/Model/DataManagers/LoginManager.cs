using MCBAWebApi.Data;
using MCBAWebApi.Models.Repository;
using MCBAWebApi.Models;

namespace MCBAWebApi.Models.DataManager;

public class LoginManager : IDataRepository<Login, string>
{
    private readonly MCBAContext _context;

    public LoginManager(MCBAContext context)
    {
        _context = context;
    }
    
    // get login from the database by id
    public Login Get(string id)
    {
        return _context.Login.Find(id);
    }
    
    // Get all logins from the database
    public IEnumerable<Login> GetAll()
    {
        return _context.Login.ToList();
    }

    // Add login to the database
    public string Add(Login login)
    {
        _context.Login.Add(login);
        _context.SaveChanges();

        return login.LoginID;
    }

    // delete login from the database
    public string Delete(string id)
    {
        _context.Login.Remove(_context.Login.Find(id));
        _context.SaveChanges();

        return id;
    }

    // Get logins that loginID contains specified string
    public IEnumerable<Login> Search(string name)
    {
        return _context.Login.Where(x => x.LoginID.Contains(name));
    } 

    // update login in the database
    public string Update(string id, Login login)
    {
        _context.Update(login);
        _context.SaveChanges();
            
        return id;
    }
}