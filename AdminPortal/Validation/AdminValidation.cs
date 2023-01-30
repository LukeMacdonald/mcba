using System.ComponentModel.DataAnnotations;

namespace AdminPortal.Validation;

public class AdminAccess : ValidationAttribute
{
    public override bool IsValid(object value) => 
        value.ToString().Equals("admin");
}