using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Du_Fang;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;

namespace Du_Fang.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(AppDBContext context, IPasswordHasher<User> passwordHasher, IConfiguration configuration) : ControllerBase
{
    private readonly AppDBContext _context = context;
    private readonly IPasswordHasher<User> _passwordHasher = passwordHasher;
    private readonly IConfiguration _configuration = configuration;

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new User
            {
                Username = model.Username,
                Email = model.Email,
                CreatedAt = DateTime.UtcNow,
                // Hash the password before saving
                PasswordHash = _passwordHasher.HashPassword(new User(), model.Password)
            };

            Console.WriteLine($"Registering user with Username: {user.Username} and Email: {user.Email}");
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { result = "User created successfully" });
        }

        [HttpPost("login")]
public async Task<IActionResult> Login([FromBody] LoginModel model)
{
    try
    {
        // Find the user by username
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == model.Username);

        // Check if user exists and password is correct
        if (user != null && !string.IsNullOrEmpty(user.PasswordHash) &&
            _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, model.Password) == PasswordVerificationResult.Success)
        {
            // Generate JWT token
            var token = GenerateJwtToken(user);

            // Return the user ID and token
            return Ok(new { UserId = user.UserId, Token = token });
        }

        // Return an error message if credentials are incorrect
        return Unauthorized(new { Message = "User credentials incorrect" });
    }
    catch (Exception ex)
    {
        // Log the exception and return a 500 Internal Server Error
        Console.Error.WriteLine($"Exception: {ex.Message}");
        return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "An error occurred while processing your request." });
    }
}

        private string GenerateJwtToken(User user)
        {
            if (user.Username == null)
            {
                throw new InvalidOperationException("UserName is null");
            }

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = _configuration["Jwt:Key"];
            if (string.IsNullOrEmpty(key))
            {
                throw new InvalidOperationException("JWT Key is not configured.");
            }

            var keyBytes = Encoding.UTF8.GetBytes(key);
            var creds = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Issuer"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // Authentication Log Endpoints (No changes needed)
        [HttpGet("logs")]
        public async Task<ActionResult<IEnumerable<Authentication_Log>>> GetAuthenticationLogs()
        {
            return await _context.AuthenticationLogs.ToListAsync();
        }

        [HttpGet("logs/{id}")]
        public async Task<ActionResult<Authentication_Log>> GetAuthenticationLog(int id)
        {
            var authentication_Log = await _context.AuthenticationLogs.FindAsync(id);

            if (authentication_Log == null)
            {
                return NotFound();
            }

            return authentication_Log;
        }

        [HttpPut("logs/{id}")]
        public async Task<IActionResult> PutAuthenticationLog(int id, Authentication_Log authentication_Log)
        {
            if (id != authentication_Log.LogId)
            {
                return BadRequest();
            }

            _context.Entry(authentication_Log).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AuthenticationLogExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpPost("logs")]
        public async Task<ActionResult<Authentication_Log>> PostAuthenticationLog(Authentication_Log authentication_Log)
        {
            _context.AuthenticationLogs.Add(authentication_Log);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAuthenticationLog", new { id = authentication_Log.LogId }, authentication_Log);
        }

        [HttpDelete("logs/{id}")]
        public async Task<IActionResult> DeleteAuthenticationLog(int id)
        {
            var authentication_Log = await _context.AuthenticationLogs.FindAsync(id);
            if (authentication_Log == null)
            {
                return NotFound();
            }

            _context.AuthenticationLogs.Remove(authentication_Log);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AuthenticationLogExists(int id)
        {
            return _context.AuthenticationLogs.Any(e => e.LogId == id);
        }
    }
}
