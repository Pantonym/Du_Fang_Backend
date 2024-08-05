using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Du_Fang;

public class Transaction
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int TransactionId { get; set; }

    [Required]
    public string? TransactionType { get; set; }

    [Required]
    [ForeignKey("FromAccount")]
    public int FromAccountId { get; set; }

    [Required]
    [ForeignKey("ToAccount")]
    public int ToAccountId { get; set; }

    [Required]
    public decimal Amount { get; set; }

    [Required]
    public DateTime Timestamp { get; set; }

    // Navigation properties
    public virtual Account ?FromAccount { get; set; }
    public virtual Account ?ToAccount { get; set; }
}