using A2Practice.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace A2Practice.Filter;

public class AuthoriseCustomerAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var customerID = context.HttpContext.Session.GetInt32(nameof(Customer.CustomerID));
        if(!customerID.HasValue)
            context.Result = new RedirectToActionResult("Login", "Login", null);
    }
    
}