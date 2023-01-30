using MCBAWebApi.Models;
using MCBAWebApi.Models.DataManager;
using Microsoft.AspNetCore.Mvc;

namespace MCBAWebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PayeeController : ControllerBase
{
    private readonly PayeeManager _repo;
    // private readonly AccountManager _accRepo;

    public PayeeController(PayeeManager repo)
    {
        _repo = repo;
    }

    // GET: api/payee
    // Gets all payees from the database
    [HttpGet]
    public IEnumerable<Payee> Get()
    {
        return _repo.GetAll();
    }


    // Gets payee with specified payID from the database
    [HttpGet("search/{name}")]
    public IEnumerable<Payee> GetSearch(int payeeID)
    {
        return _repo.Search(payeeID);
    }


    // GET api/payee/1
    // Gets payee with specified payID from the database
    [HttpGet("{id}")]
    public Payee Get(int id)
    {
        return _repo.Get(id);
    }

    // POST api/payee
    // Adds new payee to the database
    [HttpPost]
    public void Post([FromBody] Payee payee)
    {
        _repo.Add(payee);
    }

    // PUT api/payee
    // Updates payee in the database
    [HttpPut]
    public void Put([FromBody] Payee payee)
    {
        _repo.Update(payee.PayeeID, payee);
    }

    // DELETE api/payee/1
    // Deletes payee from the database
    [HttpDelete("{id}")]
    public long Delete(int id)
    {
        return _repo.Delete(id);
    }
}