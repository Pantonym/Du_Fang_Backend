using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Du_Fang.Services;
using Du_Fang;
using Isopoh.Cryptography.Argon2;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using Du_Fang.DTO;

namespace Du_Fang.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthControllerOTP : ControllerBase
    {
        private readonly EmailSender _emailSender;
        private readonly AppDBContext _context;
        private readonly IConfiguration _configuration;

        public AuthControllerOTP(AppDBContext context, EmailSender emailSender, IConfiguration configuration)
        {
            _context = context;
            _emailSender = emailSender;
            _configuration = configuration;
        }

        [HttpGet("test-email")]
        public async Task<IActionResult> TestEmail()
        {
            string toEmail = "221345@virtualwindow.co.za";
            string subject = "Test Email";
            string body = "This is a test email to verify email sending functionality.";

            await _emailSender.SendEmailAsync(toEmail, body, subject);
            return Ok("Test email sent.");
        }

        [HttpPost("generate-otp")]
        public async Task<IActionResult> GenerateOtp([FromBody] string email)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
            if (user == null) return NotFound("User not found");

            user.GenerateOTP();
            await _context.SaveChangesAsync();

            var otpMsg = $"Your OTP is {user.Otp}. It will expire in 5 min.";
            await _emailSender.SendEmailAsync(user.Email, otpMsg, "One Time Pin for Du_Fang");

            return Ok(new { message = "OTP Sent." });
        }

        [HttpPost("validate-otp")]
        public async Task<IActionResult> ValidateOtp(OtpEmail otpEmail)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == otpEmail.Email);
            if (user == null) return NotFound("User not found");

            if (user.ValidateOTP(otpEmail.Otp))
            {
                var token = GenerateJwtToken(user);
                return Ok(new { message = "OTP is valid. You are now logged in.", token });
            }
            else
            {
                return BadRequest("Invalid OTP.");
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterDto dto)
        {
            try
            {
                Console.WriteLine("Starting registration process");

                if (await _context.Users.AnyAsync(u => u.Email == dto.Email || u.Username == dto.Username))
                {
                    return BadRequest(new { message = "Email or username already taken" });
                }

                Console.WriteLine("Email and username are unique");

                if (string.IsNullOrEmpty(dto.Password))
                {
                    return BadRequest(new { message = "Password cannot be empty" });
                }

                Console.WriteLine("Password is not empty");

                var argon2 = new Argon2Hash(dto.Password);
                var hashedPassword = argon2.Hash();

                if (string.IsNullOrEmpty(hashedPassword))
                {
                    return StatusCode(500, new { message = "Password hashing failed" });
                }

                Console.WriteLine("Password hashed successfully");

                var user = new User
                {
                    Username = dto.Username,
                    Email = dto.Email,
                    CreatedAt = DateTime.UtcNow,
                    IsAdmin = false,
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                Console.WriteLine($"User created with ID: {user.UserId}");

                user.GenerateOTP();
                await _context.SaveChangesAsync();

                Console.WriteLine("OTP generated");

                var userSecurity = new User_Security
                {
                    UserId = user.UserId,
                    PasswordHash = hashedPassword,
                    LatestOTPSecret = user.Otp,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.UserSecurities.Add(userSecurity);
                await _context.SaveChangesAsync();

                Console.WriteLine("User security record created");

                var account = new Account
                {
                    UserId = user.UserId,
                    StatusId = 1,
                    Balance = 0,
                    Active = true
                };

                _context.Accounts.Add(account);
                await _context.SaveChangesAsync();

                Console.WriteLine("Account record created");

                await _emailSender.SendEmailAsync(user.Email, $"Your OTP is {user.Otp}. It will expire in 5 min.", "OTP for Du_Fang Registration");

                Console.WriteLine("OTP email sent");

                return Ok(new { message = "Registration successful. Please verify your OTP." });
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"Database error during registration: {ex.Message}");
                Console.WriteLine($"Inner exception: {ex.InnerException?.Message}");
                return StatusCode(500, new { message = $"A database error occurred during registration: {ex.InnerException?.Message ?? ex.Message}" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during registration: {ex}");
                return StatusCode(500, new { message = $"An error occurred during registration: {ex.Message}" });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDto dto)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null)
                return Unauthorized(new { message = "Invalid email or password" });

            var userSecurity = await _context.UserSecurities.SingleOrDefaultAsync(us => us.UserId == user.UserId);
            if (userSecurity == null)
                return Unauthorized(new { message = "Invalid email or password" });

            if (!VerifyPassword(dto.Password, userSecurity.PasswordHash))
                return Unauthorized(new { message = "Invalid email or password" });

            var token = GenerateJwtToken(user);

            // Log authentication details
            var authLog = new Authentication_Log
            {
                UserId = user.UserId,
                LoginTime = DateTime.UtcNow,
                IP_Address = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
                DeviceInfo = HttpContext.Request.Headers["User-Agent"].ToString()
            };

            _context.AuthenticationLogs.Add(authLog);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Login successful", token, user.UserId });
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.IsAdmin ? "Admin" : "User")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(Convert.ToDouble(_configuration["Jwt:ExpireDays"]));

            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Issuer"],
                claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private bool VerifyPassword(string plainPassword, string hashedPassword)
        {
            return Argon2.Verify(hashedPassword, plainPassword);
        }

        public class OtpEmail
        {
            public string Email { get; set; }
            public string Otp { get; set; }
        }
    }
}