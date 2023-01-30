using MCBAWebApi.Models;
using MCBAWebApi.Models.DataManager;
using Microsoft.AspNetCore.Mvc;

namespace MCBAWebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BillPayController : ControllerBase
{
    private readonly BillPayManager _repo;

    private readonly AccountController _accountController;

    public BillPayController(BillPayManager repo)
    {
        _repo = repo;
    }

    // GET: api/billpay
    // Gets all bill pays from the database
    [HttpGet]
    public IEnumerable<BillPay> Get()
    {
        return _repo.GetAll();
    }


    // Gets all bill pays with specified payID from database
    [HttpGet("search/{name}")]
    public IEnumerable<BillPay> GetSearch(int payID)
    {
        return _repo.Search(payID);
    }


    // GET api/billpay/1
    // Gets specified billpay from database
    [HttpGet("{id}")]
    public BillPay Get(int id)
    {
        return _repo.Get(id);
    }

    // POST api/billpay
    // Adds new billpay to the database
    [HttpPost]
    public void Post([FromBody] BillPay billPay)
    {
        _repo.Add(billPay);
    }

    // PUT api/billpay
    // Updates billpay in the database
    [HttpPut]
    public void Put([FromBody] BillPay billPay)
    {
        _repo.Update(billPay.BillPayID, billPay);
    }

    // DELETE api/billpay/1
    // Deletes billpay from the database
    [HttpDelete("{id}")]
    public long Delete(int id)
    {
        return _repo.Delete(id);
    }
}