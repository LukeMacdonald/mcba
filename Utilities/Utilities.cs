namespace Utilities;

public static class Utilities
{
    // Below Code Copied from Day 5 Tutorial Code
    private static bool HasNDecimalPlaces(this decimal value, int n) => 
        decimal.Round(value, n).ToString() == value.ToString();
   

    public static bool HasTwoDecimalPlaces(this decimal value) => 
        value.HasNDecimalPlaces(2);

    public static bool IsPositive(this decimal value) => 
        value > Decimal.Zero;
    
    public static bool HasDigitLengthN(this int value, int n) => value.ToString().Length == n;

    public static bool HasDigitLength8(this int value) => HasDigitLengthN(value, 8);
    
    public static bool HasDigitLength4(this int value) => HasDigitLengthN(value, 4);
}