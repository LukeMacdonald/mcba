using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MCBAWebApi.Enum;

namespace MCBAWebApi.Models;

public class Customer
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int CustomerID { get; set; }
    
    [Required, StringLength(50)]
    public string Name { get; set; }
    
    [StringLength(50)]
    public string?  Address { get; set; }
    
    [StringLength(40)]
    public string?  City { get; set; }
    
    public AustralianState? State { get; set; }
    
    [StringLength(4, MinimumLength = 4)]
    public string? Postcode { get; set; }
    
    [StringLength(12)]
    public string? Mobile { get; set; }
    
    [StringLength(12)]
    public string? TFN { get; set; }
    
    public virtual List<Account> Accounts { get; set; }
}