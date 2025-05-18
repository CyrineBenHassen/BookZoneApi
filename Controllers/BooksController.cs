using BookZoneAPI.Data;
using BookZoneAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookZoneAPI.Controllers
{
    [ApiController]
    [Route("api/books")]
    public class BooksController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BooksController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var books = await _context.Books
                .Include(b => b.Author)
                .Include(b => b.Category)
                .ToListAsync();

            return Ok(books);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var book = await _context.Books
                .Include(b => b.Author)
                .Include(b => b.Category)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (book == null)
                return NotFound();

            return Ok(book);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Book book)
        {
            if (book == null)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Attacher uniquement les entités liées existantes via leurs IDs
            if (book.AuthorId != 0)
            {
                _context.Authors.Attach(new Author { Id = book.AuthorId });
            }
            else
            {
                return BadRequest("AuthorId is required.");
            }

            if (book.CategoryId != 0)
            {
                _context.Categories.Attach(new Category { Id = book.CategoryId });
            }
            else
            {
                return BadRequest("CategoryId is required.");
            }

            // Ne pas inclure les objets author et category dans book
            book.Author = null;
            book.Category = null;

            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = book.Id }, book);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Book book)
        {
            if (id != book.Id)
                return BadRequest("ID mismatch");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Attacher entités liées si besoin
            if (book.AuthorId != 0)
            {
                _context.Authors.Attach(new Author { Id = book.AuthorId });
            }
            else
            {
                return BadRequest("AuthorId is required.");
            }

            if (book.CategoryId != 0)
            {
                _context.Categories.Attach(new Category { Id = book.CategoryId });
            }
            else
            {
                return BadRequest("CategoryId is required.");
            }

            book.Author = null;
            book.Category = null;

            _context.Entry(book).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Books.Any(b => b.Id == id))
                    return NotFound();
                throw;
            }

            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
                return NotFound();

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
