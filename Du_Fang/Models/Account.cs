using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Du_Fang;

public class Account
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int AccountId { get; set; }

    [Required]
    [ForeignKey("User")]
    public int UserId { get; set; }

[Required]
    [ForeignKey("Status")]
    public int StatusId { get; set; } 

    [Required]
    public decimal Balance { get; set; }

    [Required]
    public int CoinBalance { get; set; }

    public bool Active { get; set; }

    // Navigation properties
    public virtual User? User { get; set; }
    public virtual Status? Status { get; set; }
    public virtual ICollection<Transaction>? TransactionsFrom { get; set; }
    public virtual ICollection<Transaction>? TransactionsTo { get; set; }
}