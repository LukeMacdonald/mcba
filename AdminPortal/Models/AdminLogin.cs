using AdminPortal.Validation;

namespace AdminPortal.Models;

public class AdminLogin
{
    [AdminAccess]
    public string Username { get; set; }
    
    [AdminAccess]
    public string Password { get; set; }
}