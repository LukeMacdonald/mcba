using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdminPortal.Models;

public class CustomerLogin
{
    [Column(TypeName = "char")]
    [StringLength(8)]
    public string LoginID { get; set; }

    public int CustomerID { get; set; }
    public virtual CustomerDto Customer { get; set; }

    [Column(TypeName = "char")]
    [Required, StringLength(94)]
    public string PasswordHash { get; set; }

    [Required]
    public bool Disabled { get; set; }
}