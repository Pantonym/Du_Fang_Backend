using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Du_Fang;

public class Status
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int StatusId { get; set; }

[Required]
    public string StatusName { get; set; }

    public decimal TotalAmountCriteria { get; set; }

    public int TransactionsCriteria { get; set; }

    public double AnnualInterestRate { get; set; }

    public decimal TransactionFee { get; set; }

    // Navigation property
    public virtual ICollection<Account> ?Accounts { get; set; }
}