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
    public class AuthLogController : ControllerBase
    {
        private readonly AppDBContext _context;

        public AuthLogController(AppDBContext context)
        {
            _context = context;
        }

        // GET: api/AuthLog
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Authentication_Log>>> GetAuthenticationLogs()
        {
            return await _context.AuthenticationLogs.ToListAsync();
        }

        // GET: api/AuthLog/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Authentication_Log>> GetAuthentication_Log(int id)
        {
            var authentication_Log = await _context.AuthenticationLogs.FindAsync(id);

            if (authentication_Log == null)
            {
                return NotFound();
            }

            return authentication_Log;
        }

        // PUT: api/AuthLog/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAuthentication_Log(int id, Authentication_Log authentication_Log)
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
                if (!Authentication_LogExists(id))
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

        // POST: api/AuthLog
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Authentication_Log>> PostAuthentication_Log(Authentication_Log authentication_Log)
        {
            _context.AuthenticationLogs.Add(authentication_Log);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAuthentication_Log", new { id = authentication_Log.LogId }, authentication_Log);
        }

        // DELETE: api/AuthLog/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAuthentication_Log(int id)
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

        private bool Authentication_LogExists(int id)
        {
            return _context.AuthenticationLogs.Any(e => e.LogId == id);
        }
    }
}
