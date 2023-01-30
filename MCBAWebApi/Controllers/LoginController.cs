using MCBAWebApi.Models;
using MCBAWebApi.Models.DataManager;
using Microsoft.AspNetCore.Mvc;

namespace MCBAWebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LoginController
{
    private readonly LoginManager _repo;

    public LoginController(LoginManager repo)
    {
        _repo = repo;
    }

    // GET: api/login
    // Gets all logins from the database
    [HttpGet]
    public IEnumerable<Login> Get()
    {
        return _repo.GetAll();
    }


    // Gets all logins with specified id from the database
    [HttpGet("search/{name}")]
    public IEnumerable<Login> GetSearch(string name)
    {
        return _repo.Search(name);
    }
 

    // GET api/login/1
    // Gets all logins with specified id from the database
    [HttpGet("{id}")]
    public Login Get(string id)
    {
        return _repo.Get(id);
    }

    // POST api/login
    // Adds a new login to the database
    [HttpPost]
    public void Post([FromBody] Login login)
    {
        _repo.Add(login);
    }

    // PUT api/login
    // Updates login in the database
    [HttpPut]
    public void Put([FromBody] Login login)
    {
        _repo.Update(login.LoginID, login);
    }

    // DELETE api/login/1
    // Deletes login from the database
    [HttpDelete("{id}")]
    public string Delete(string id)
    {
        return _repo.Delete(id);
    }
}