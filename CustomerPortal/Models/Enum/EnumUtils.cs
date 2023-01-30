namespace CustomerPortal.Models.Enum;

public static class EnumUtils
{
    public static AccountType ConvertAccountType(char type)
    {
        switch (type)
        {
            case 'C':
                return AccountType.Checking;
            case 'S':
                return AccountType.Saving;
            default:
                throw new Exception("Invalid AccountType");
        }
    }
    public static TransactionType ConvertTransactionType(char type)
    {
        switch (type)
        {
            case 'D':
                return TransactionType.Deposit;
            case 'T':
                return TransactionType.Transfer;
            case 'W':
                return TransactionType.Withdraw;
            case 'S':
                return TransactionType.ServiceCharge;
            case 'B':
                return TransactionType.BillPay;
            default:
                throw new Exception("Invalid TransactionType");
        }
    }
    public static AustralianState ConvertState(string state)
    {
        switch (state)
        {
            case "NSW":
                return AustralianState.NewSouthWales;
            case "VIC":
                return AustralianState.Victoria;
            case "ACT":
                return AustralianState.AustralianCapitalTerritory;
            case "NT":
                return AustralianState.NorthernTerritory;
            case "WA":
                return AustralianState.WesternAustralia;
            case "SA":
                return AustralianState.SouthAustralia;
            case "QLD":
                return AustralianState.Queensland;
            case "TAS":
                return AustralianState.Tasmania;
            default:
                throw new Exception("Invalid State");
        }
        
    }
    public static string ReverseConvertState(AustralianState? state)
    {
        switch (state)
        {
            case AustralianState.NewSouthWales:
                return "NSW";
            case AustralianState.Victoria:
                return "VIC";
            case AustralianState.AustralianCapitalTerritory:
                return "ACT";
            case AustralianState.NorthernTerritory:
                return "NT";
            case AustralianState.WesternAustralia:
                return "WA";
            case AustralianState.SouthAustralia:
                return "SA";
            case AustralianState.Queensland:
                return "QLD";
            case AustralianState.Tasmania:
                return "TAS";
            default:
                return null;
        }
        
    }
    
}