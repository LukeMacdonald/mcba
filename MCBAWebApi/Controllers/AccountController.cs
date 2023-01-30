using MCBAWebApi.Models;
using MCBAWebApi.Models.DataManager;
using Microsoft.AspNetCore.Mvc;

namespace MCBAWebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly AccountManager _repo;
    // private readonly AccountManager _accRepo;

    public AccountController(AccountManager repo)
    {
        _repo = repo;
    }

    // GET: api/customer
    // Gets all customers from the database
    [HttpGet]
    public IEnumerable<Account> Get()
    {
        return _repo.GetAll();
    }


    // Gets accounts by account number
    [HttpGet("search/{name}")]
    public IEnumerable<Account> GetSearch(int accNumber)
    {
        return _repo.Search(accNumber);
    }

    
    // GET api/customer/1
    // Gets accounts by account number
    [HttpGet("{id}")]
    public Account Get(int id)
    {
        return _repo.Get(id);
    }

    // POST api/account
    // Adds a new account to the database
    [HttpPost]
    public void Post([FromBody] Account account)
    {
        _repo.Add(account);
    }

    // PUT api/account
    // Updates account in databse
    [HttpPut]
    public void Put([FromBody] Account account)
    {
        _repo.Update(account.AccountNumber, account);
    }

    // DELETE api/account/1
    // Deletes account from database
    [HttpDelete("{id}")]
    public long Delete(int id)
    {
        return _repo.Delete(id);
    }
}