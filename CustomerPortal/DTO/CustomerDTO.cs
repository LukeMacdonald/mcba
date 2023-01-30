namespace CustomerPortal.DTO;

public class CustomerDTO
{
    public int CustomerID {get;set;}
    public string Name {get;set;}
    public string Address {get;set;}
    public string City {get;set;}
    public string Postcode {get;set;}
    public LoginDTO Login {get;set;}
    public List<AccountDTO> Accounts {get;set;}
}