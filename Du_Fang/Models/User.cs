using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Du_Fang
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }

        [Required]
        public string? Username { get; set; }

        [Required]
        public string? Email { get; set; }

        public bool IsAdmin { get; set; }


        public DateTime CreatedAt { get; set; }

        // Navigation property for the one-to-one relationship with User_Security
        public virtual User_Security? UserSecurity { get; set; }

        // Navigation property for the one-to-many relationship with Authentication_Log
        public virtual ICollection<Authentication_Log>? AuthenticationLogs { get; set; }

        // Navigation property for the one-to-one relationship with Account
        public virtual Account? Account { get; set; }

        // OTP
        public string? Otp { get; set; }

        public DateTime? OtpExpiry { get; set; }

        //method to control our otp
        public void GenerateOTP()
        {
            var random = new Random();
            Otp = random.Next(100000, 999999).ToString();
            OtpExpiry = DateTime.UtcNow.AddMinutes(5);
        }

        public bool ValidateOTP(string receivedOTP)
        {
            return Otp == receivedOTP && OtpExpiry > DateTime.UtcNow;

        }
    }
}
