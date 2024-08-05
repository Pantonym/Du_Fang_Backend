using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Du_Fang;

public class User_Security
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int SecurityId { get; set; }

    [Required]
    [ForeignKey("User")]
    public int UserId { get; set; }

    [Required]
    public string? PasswordHash { get; set; }

    public string? LatestOTPSecret { get; set; }

    public DateTime UpdatedAt { get; set; }

    // Navigation property:
    // Creates a proxy object by using the virtual keyword, which allows the program to utilise lazy loading to easily access it. 
    // Lazy Loading stops an object from loading until it is needed.
    // This is for future implementation, as Entity Framework will have to be implemented to support this to its full extent.
    // Currently, this si used for the on-to-one relationship between this and the User model
    public virtual User? User { get; set; }
}