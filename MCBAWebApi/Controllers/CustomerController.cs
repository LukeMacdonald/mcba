using MCBAWebApi.Models;
using MCBAWebApi.Models.DataManager;
using Microsoft.AspNetCore.Mvc;
namespace MCBAWebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomerController : ControllerBase
{
    private readonly CustomerManager _repo;
    // private readonly AccountManager _accRepo;

    public CustomerController(CustomerManager repo)
    {
        _repo = repo;
    }

    // GET: api/customer
    // Gets all customers from the database
    [HttpGet]
    public IEnumerable<Customer> Get()
    {
        var customers = _repo.GetAll();
        return customers;
    }

    // Gets customers with name containing 'name' in the database
    [HttpGet("search/{name}")]
    public IEnumerable<Customer> GetSearch(string name)
    {
        return _repo.Search(name);
    }
 

    // GET api/customer/1
    // Gets customer with specified customerID from the database
    [HttpGet("{id}")]
    public Customer Get(int id)
    {
        return _repo.Get(id);
    }

    // POST api/customer
    // Add new customer to the database
    [HttpPost]
    public void Post([FromBody] Customer customer)
    {
        _repo.Add(customer);
    }

    // PUT api/customer
    // Updates customer in the database
    [HttpPut]
    public void Put([FromBody] Customer customer)
    {
        _repo.Update(customer.CustomerID, customer);
    }

    // DELETE api/customer/1
    // Deletes customer from the database
    [HttpDelete("{id}")]
    public long Delete(int id)
    {
        return _repo.Delete(id);
    }
}