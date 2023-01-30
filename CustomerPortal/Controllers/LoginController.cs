using CustomerPortal.Data;
using CustomerPortal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleHashing.Net;

namespace A2Practice.Controllers;
[Route("/MCBA/Login")]
public class LoginController : Controller
{
    private static readonly ISimpleHash SimpleHash = new SimpleHash();
    private readonly MCBAContext _context;

    public LoginController(MCBAContext context) => _context = context;

    public IActionResult Index() => View();
    
    // Method For 
    [HttpPost]
    public async Task<IActionResult> Index(string loginID, string password)
    {
        var login = await _context.Login.Include(x=>x.Customer).
            FirstOrDefaultAsync(x=> x.LoginID == loginID);
        
        // Checks that all input fields are valid
        if (login == null || string.IsNullOrEmpty(password) || !SimpleHash.Verify(password, login.PasswordHash))
        {
            ModelState.AddModelError("LoginFailed","Login Failed. Please Try Again!");
            return View(new Login { LoginID = loginID });
        }

        // If Login Is diasbled by admin than an error message is disaplyed to user and they cannot 
        // proceed even with the correct details
        if (login.Disabled)
        {
            ModelState.AddModelError("LoginFailed","Your account is locked.");
            return View(new Login { LoginID = loginID });
        }
        
        // Login Customer
        HttpContext.Session.SetInt32(nameof(Customer.CustomerID),login.CustomerID);
        HttpContext.Session.SetString(nameof(Customer.Name),login.Customer.Name);

        return RedirectToAction("Index", "Customer");
        
    }

    // Method to return user to the landing page and 
    // clears all the data from the session
    [Route("Logout")]
    public IActionResult Logout()
    {
        // Clears the HTTP Session
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Home");
        
    }
    
}