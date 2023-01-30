using CustomerPortal.Filter;
using CustomerPortal.Data;

using CustomerPortal.Models;
using CustomerPortal.Models.ViewModels;
using ImageMagick;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SimpleHashing.Net;

namespace CustomerPortal.Controllers;

// Can add authorize attribute to controllers.
[AuthoriseCustomer]
public class CustomerController : Controller
{
    
    private readonly MCBAContext _context;
    
    // Loads the CustomerID entered at Login
    private static readonly ISimpleHash SimpleHash = new SimpleHash();
    public int CustomerID => HttpContext.Session.GetInt32(nameof(Customer.CustomerID)).Value;

    public CustomerController(MCBAContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var customer = await FindCustomer(CustomerID);
        if (!customer.DisplayPicture.IsNullOrEmpty())
        {
            HttpContext.Session.SetString(nameof(Customer.DisplayPicture), customer.DisplayPicture);
        }
        else
        {
            HttpContext.Session.SetString(nameof(Customer.DisplayPicture), "default");
        }
        return View(customer);
    }
    
    // Method for returning view which shows the user info in read-only format
    public async Task<IActionResult> Profile()
    {
        var customer = await FindCustomer(CustomerID);
        return View(customer);
    }
    
    // Method for returning view which allows user to update profile database
    public async Task<IActionResult> UpdateProfile()
    {
        var customer = await FindCustomer(CustomerID);
        return View(customer);
    }

    // Post/Put Method for updating customer profile
    [HttpPost]
    public async Task<IActionResult> UpdateProfile(Customer customer)
    {
        if (!ModelState.IsValid)
        {
            return View(customer);
        }

        _context.Customer.Update(customer);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
    
    // Method for returning view which allows user to set new password
    public async Task<IActionResult> NewPassword()
    {
        var customer = await _context.Customer.Include(x => x.Login)
            .FirstOrDefaultAsync(x => x.CustomerID == CustomerID);

        return View(new LoginViewModel()
        {
            LoginID = customer.Login.LoginID,
            PasswordHash = customer.Login.PasswordHash
        });
    }

    // Post/Put Method for setting new password
    [HttpPost]
    public async Task<IActionResult> NewPassword(LoginViewModel login)
    {
        var customer = await _context.Login.FindAsync(login.LoginID);

        if (!ModelState.IsValid)
        {
            return View(login);
        }

        // Gets the Hash of new Password
        customer.PasswordHash = SimpleHash.Compute(login.NewPassword);

        // Updates the login table with new password
        _context.Login.Update(customer);

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Profile));
    }
    
    // Post Method for Setting New Profile Picture
    [HttpPost]
    public async Task<IActionResult> Profile(Customer customer)
    {

        var c = await FindCustomer(CustomerID);
        
        IFormFile ImageFile = customer.ImageFile;
        
        if (ImageFile != null)
        {
            try
            {
                using var dataStream = new MemoryStream();
                // Gets MagicImage object from the Selected Image
                using var image = new MagickImage(ImageFile.OpenReadStream());
                // Defines the size of image (400x400)
                var size = new MagickGeometry(400, 400);
                size.IgnoreAspectRatio = false;
                // Sets Format of Image to Jpeg
                image.Format = MagickFormat.Jpeg;
                // Resizes the image
                image.Resize(size);
                
                // Converts the image to bytes and stores the image as a string 
                // in the database
                await image.WriteAsync(dataStream);
                var imageBytes = dataStream.ToArray();
                var base64String = Convert.ToBase64String(imageBytes);
                c.DisplayPicture = base64String;
                
            }
            // Exception is called when a non image file is selected and the user is returned to
            // the profile view without updating the profile pic
            catch (Exception e)
            {
                ModelState.AddModelError("ImageFile","Incorrect File Input!");
                return View(c);
            }
        }
        _context.Customer.Update(c);
        await _context.SaveChangesAsync();
        HttpContext.Session.SetString(nameof(Customer.DisplayPicture), c.DisplayPicture);
        return View(c);
    }

    // Method for Deleting Customer Profile Pic
    public async Task<IActionResult> DeletePicture()
    {
        var customer = await FindCustomer(CustomerID);
        // Sets all the Fields for Image to null
        customer.DisplayPicture = null;
        customer.ImageFile = null;
        _context.Customer.Update(customer);
        // Saves changes to database
        await _context.SaveChangesAsync();
        // Sets the SessionString used to represent the image to the default image.
        HttpContext.Session.SetString(nameof(Customer.DisplayPicture), "default");
        return RedirectToAction(nameof(Profile));
    }
    public async Task<Customer> FindCustomer(int customerID)
    {
        return await _context.Customer.Include(x => x.Accounts).Include(x=>x.Login)
            .FirstOrDefaultAsync(x => x.CustomerID == customerID);
    }
}