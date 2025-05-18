using BookZoneAPI.Data;
using BookZoneAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookZoneAPI.Controllers
{
    [ApiController]
    [Route("api/authors")]
    public class AuthorsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public AuthorsController(ApplicationDbContext context) => _context = context;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var authors = await _context.Authors.ToListAsync();
            return Ok(authors);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var author = await _context.Authors.FindAsync(id);
            if (author == null)
                return NotFound(new { message = $"Author with ID {id} not found." });

            return Ok(author);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Author author)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.Authors.Add(author);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = author.Id }, author);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Author updatedAuthor)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var author = await _context.Authors.FindAsync(id);
            if (author == null)
                return NotFound(new { message = $"Author with ID {id} not found." });

            // Mise à jour contrôlée
            author.Name = updatedAuthor.Name;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var author = await _context.Authors.FindAsync(id);
            if (author == null)
                return NotFound(new { message = $"Author with ID {id} not found." });

            _context.Authors.Remove(author);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
