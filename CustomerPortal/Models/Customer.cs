

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CustomerPortal.Models.Enum;
using JetBrains.Annotations;

namespace CustomerPortal.Models;

public class Customer
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int CustomerID { get; set; }
    
    [Required, StringLength(50)]
    public string Name { get; set; }
    
    [StringLength(50)] public string?  Address { get; set; }
    
    [StringLength(40)] public string? City { get; set; }
    
    [CanBeNull]
    public AustralianState? State { get; set; }
    
    [StringLength(4, MinimumLength = 4)] public string? Postcode { get; set; }
    
    [StringLength(12)]
    [RegularExpression("[0][4][0-9]{2} [0-9]{3} [0-9]{3}",ErrorMessage = "Must be in format: 04XX XXX XXX")]
    public string? Mobile { get; set; }
    
    [StringLength(11)]
    [RegularExpression("[0-9]{3} [0-9]{3} [0-9]{3}",ErrorMessage = "Must be in format: XXX XXX XXX")]
    public string? TFN { get; set; }
    
    // public virtual ICollection<Account> Accounts { get; set; }
    public virtual ICollection<Account> Accounts { get; } = new List<Account>();
    public virtual Login Login { get; set; }
    
    [NotMapped]
    // [Required(ErrorMessage = "Please select an image to upload.")]
    public IFormFile? ImageFile { get; set; }
    
    public string? DisplayPicture { get; set; }
}