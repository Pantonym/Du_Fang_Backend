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

        [Required]
        public string? PasswordHash { get; set; }

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
            //6 digits otp
            var random = new Random();
            Otp = random.Next(100000, 999999).ToString(); // generate 6-digit otp
                                                          //TODO SELF: encrypt the OTP - Argon2 (own research)
            OtpExpiry = DateTime.UtcNow.AddMinutes(5); // 5 minutes expiry
        }

        public bool ValidateOTP(string receivedOTP)
        {
            // check if the received OTP/emailed is valid/match, and are we still in the 5 minute expiry window
            return Otp == receivedOTP && OtpExpiry > DateTime.UtcNow;

            // TODO self (optional): generate the JWT token

        }
    }
}
