using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AdminPortal.Enum;
using JetBrains.Annotations;


namespace AdminPortal.Models;

public class CustomerDto
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int CustomerID { get; set; }
    
    [Required, StringLength(50)]
    public string Name { get; set; }
    
    [StringLength(50)]
    
    [CanBeNull]
    public string  Address { get; set; }
    
    [StringLength(40)]
    [CanBeNull]
    public string City { get; set; }
    
    [CanBeNull]
    public AustralianState State { get; set; }
    
    [StringLength(4, MinimumLength = 4)]
    [CanBeNull]
    public string Postcode { get; set; }
    
    [StringLength(12)]
    [CanBeNull]
    [RegularExpression("[0][4][0-9]{2} [0-9]{3} [0-9]{3}",ErrorMessage = "Must be in format: 04XX XXX XXX")]
    public string Mobile { get; set; }
    
    [StringLength(11)]
    [CanBeNull]
    [RegularExpression("[0-9]{3} [0-9]{3} [0-9]{3}",ErrorMessage = "Must be in format: XXX XXX XXX")]
    public string TFN { get; set; }
    
    // public virtual ICollection<Account> Accounts { get; set; }
    public virtual ICollection<AccountDto> Accounts { get; } = new List<AccountDto>();
    // public virtual Login Login { get; set; }

    [CanBeNull] 
    public string DisplayPicture { get; set; }
    
    [NotMapped]
    public bool Locked { get; set; }
}