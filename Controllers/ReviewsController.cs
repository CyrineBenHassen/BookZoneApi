using BookZoneAPI.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Authorize]
[ApiController]
[Route("api/reviews")]
public class ReviewsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public ReviewsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpPost("books/{bookId}")]
    public async Task<IActionResult> PostReview(int bookId, [FromBody] Review review)
    {
        var userId = _userManager.GetUserId(User);
        if (userId == null)
            return Unauthorized();

        review.UserId = userId;
        review.BookId = bookId;
        review.DatePosted = DateTime.UtcNow;

        _context.Reviews.Add(review);
        await _context.SaveChangesAsync();

        return Ok(review);
    }

    [HttpGet("books/{bookId}")]
    public async Task<IActionResult> GetReviews(int bookId)
    {
        var reviews = await _context.Reviews.Where(r => r.BookId == bookId).ToListAsync();
        if (reviews.Count == 0)
            return NotFound("Aucun avis pour ce livre.");

        var averageRating = reviews.Average(r => r.Rating);
        return Ok(new { AverageRating = averageRating, Reviews = reviews });
    }
}
