using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Du_Fang;

namespace Du_Fang.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserSecurityController : ControllerBase
    {
        private readonly AppDBContext _context;

        public UserSecurityController(AppDBContext context)
        {
            _context = context;
        }

        // GET: api/UserSecurity
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User_Security>>> GetUserSecurities()
        {
            return await _context.UserSecurities.ToListAsync();
        }

        // GET: api/UserSecurity/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User_Security>> GetUser_Security(int id)
        {
            var user_Security = await _context.UserSecurities.FindAsync(id);

            if (user_Security == null)
            {
                return NotFound();
            }

            return user_Security;
        }

        // PUT: api/UserSecurity/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser_Security(int id, User_Security user_Security)
        {
            if (id != user_Security.SecurityId)
            {
                return BadRequest();
            }

            _context.Entry(user_Security).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!User_SecurityExists(id))
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

        // POST: api/UserSecurity
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<User_Security>> PostUser_Security(User_Security user_Security)
        {
            _context.UserSecurities.Add(user_Security);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser_Security", new { id = user_Security.SecurityId }, user_Security);
        }

        // DELETE: api/UserSecurity/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser_Security(int id)
        {
            var user_Security = await _context.UserSecurities.FindAsync(id);
            if (user_Security == null)
            {
                return NotFound();
            }

            _context.UserSecurities.Remove(user_Security);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool User_SecurityExists(int id)
        {
            return _context.UserSecurities.Any(e => e.SecurityId == id);
        }
    }
}
