using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Du_Fang
{
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

    public class RegisterModel
    {
        [Required]
        public required string Username { get; set; }

        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        [MinLength(6)]
        public required string Password { get; set; }
    }

    public class LoginModel
    {
        [Required]
        public required string Username { get; set; }

        [Required]
        [MinLength(6)]
        public required string Password { get; set; }
    }
}
