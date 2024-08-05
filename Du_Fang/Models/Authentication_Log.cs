using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Du_Fang;

public class Authentication_Log
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int LogId { get; set; }

    [Required]
    [ForeignKey("User")]
    public int UserId { get; set; }

    [Required]
    public DateTime LoginTime { get; set; }

    public DateTime? LogOutTime { get; set; }

    [Required]
    [MaxLength(45)] // IPv6 max length is 45 characters
    public string? IP_Address { get; set; }

    [MaxLength(256)] // To prevent excessive descriptions
    public string? DeviceInfo { get; set; }

    // Navigation property to access the related User
    public virtual User? User { get; set; }
}