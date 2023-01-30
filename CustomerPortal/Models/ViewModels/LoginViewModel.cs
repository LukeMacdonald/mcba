using System.ComponentModel.DataAnnotations;
using SimpleHashing.Net;

namespace CustomerPortal.Models.ViewModels;

public class LoginViewModel : IValidatableObject
{
    // Stored Login Values
    public string LoginID { get; set; }
    public string PasswordHash { get; set; }
    
    // Values entered by Users
    public string OldPassword { get; set;}
    public string NewPassword { get; set;}
    public string ConfirmPassword { get; set;}

    // ViewModels Validation Check
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    { 
        ISimpleHash simpleHash = new SimpleHash();
        
        if (!simpleHash.Verify(OldPassword, PasswordHash))
        {
            yield return new ValidationResult(
                $"Old Password Did Not Match!",
                new[] { nameof(OldPassword) });
        }
        if (!NewPassword.Equals(ConfirmPassword))
        {
            yield return new ValidationResult(
                $"Passwords Don't Match!",
                new[] { nameof(ConfirmPassword) });
        }
        
        if (simpleHash.Verify(NewPassword, PasswordHash))
        {
            yield return new ValidationResult(
                $"Password Must Be Different!",
                new[] { nameof(NewPassword) });
        }
        
    }
}