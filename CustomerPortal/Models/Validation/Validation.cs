using System.ComponentModel.DataAnnotations;
using Utilities;

namespace CustomerPortal.Models.Validation;

public class TwoDecimalPlaces : ValidationAttribute
{
    public override bool IsValid(object value) => 
        Convert.ToDecimal(value).HasTwoDecimalPlaces();

    public override string FormatErrorMessage(string name)
    {
        return "Value Must be in the Format of 2 Decimal Place.";
    }
}

public class IsPositive : ValidationAttribute
{
    public override bool IsValid(object value) => 
        Convert.ToDecimal(value).IsPositive();
    
    public override string FormatErrorMessage(string name)
    {
        return "Value Must be Greater Than 0";
    }
}
public class AccountNumberLength : ValidationAttribute
{
    public override bool IsValid(object value) => 
        Convert.ToInt32(value).HasDigitLength4();
    public override string FormatErrorMessage(string name)
    {
        return "Account Number must be 4 Digits";
    }
}