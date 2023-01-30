using System.ComponentModel.DataAnnotations;
namespace CustomerPortal.Models;
using Enum;

public class Payee
{
    public int PayeeID { get; set; }
    
    [Required,StringLength(30)]
    public string Name { get; set; }
    
    [Required,StringLength(50)]
    public string? Address { get; set; }
    
    [Required,StringLength(40)]
    public string? City { get; set; }
    
    [Required]
    public AustralianState State { get; set; }
    
    [Required,StringLength(4, MinimumLength = 4)]
    public string Postcode { get; set; }
    
    [Required,StringLength(12)]
    [RegularExpression("[0][4][0-9]{2} [0-9]{3} [0-9]{3}",ErrorMessage = "Must be in format: 04XX XXX XXX")]
    public string Mobile { get; set; }
    
}